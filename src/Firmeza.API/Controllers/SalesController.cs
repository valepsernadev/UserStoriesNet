using System.Security.Claims;
using AutoMapper;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Firmeza.API.DTOs;
using Firmeza.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController(
  ISaleService saleService,
  IMapper mapper,
  IClientService clientService,
  IPdfService pdfService,
  IEmailService emailService,
  ILogger<SalesController> logger) : ControllerBase
{
  /// <summary>Lista todas las ventas. Solo Administrador.</summary>
  [HttpGet]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> GetAll()
  {
    var sales = await saleService.GetAllAsync();
    return Ok(mapper.Map<List<SaleDto>>(sales));
  }

  /// <summary>Obtiene una venta por ID. Cliente propietario o Administrador.</summary>
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    var sale = await saleService.GetByIdAsync(id);

    if (sale is null)
      return NotFound();

    if (User.IsInRole("Cliente") && !await IsOwner(sale))
      return Forbid();

    return Ok(mapper.Map<SaleDto>(sale));
  }

  /// <summary>Crea una venta y envía recibo por email. Cliente autenticado o Administrador.</summary>
  [HttpPost]
  public async Task<IActionResult> Create(CreateSaleDto dto)
  {
    int clientId;
    string clientEmail;
    string clientName;

    if (User.IsInRole("Cliente"))
    {
      var userEmail = User.FindFirstValue(ClaimTypes.Email)!;
      var client = await clientService.GetByEmailAsync(userEmail);

      if (client is null)
        return BadRequest(new { error = "No se encontró un cliente asociado a este usuario. Regístrese primero como cliente en POST /api/clients." });

      clientId = client.Id;
      clientEmail = client.Email;
      clientName = client.FullName;
    }
    else
    {
      if (dto.ClientId is null)
        return BadRequest(new { error = "ClientId es obligatorio para administradores." });

      var client = await clientService.GetByIdAsync(dto.ClientId.Value);

      if (client is null)
        return NotFound(new { error = "Cliente no encontrado." });

      clientId = client.Id;
      clientEmail = client.Email;
      clientName = client.FullName;
    }

    var items = dto.Items
      .Select(i => new SaleItemInput { ProductId = i.ProductId, Quantity = i.Quantity })
      .ToList();

    Sale sale;
    try
    {
      sale = await saleService.CreateAsync(clientId, items);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(new { error = ex.Message });
    }

    var fullSale = await saleService.GetByIdAsync(sale.Id);

    if (fullSale is not null)
    {
      try
      {
        var receiptData = BuildReceiptData(fullSale);
        var pdf = pdfService.GenerateReceipt(receiptData);
        await emailService.SendReceiptAsync(clientEmail, clientName, sale.Id, pdf);
      }
      catch (Exception ex)
      {
        logger.LogWarning(ex, "No se pudo enviar el recibo por email para la venta {SaleId}", sale.Id);
      }
    }

    return CreatedAtAction(nameof(GetById), new { id = sale.Id }, mapper.Map<SaleDto>(fullSale));
  }

  /// <summary>Descarga el recibo PDF de una venta. Cliente propietario o Administrador.</summary>
  [HttpGet("{id:int}/receipt")]
  public async Task<IActionResult> GetReceipt(int id)
  {
    var sale = await saleService.GetByIdAsync(id);

    if (sale is null)
      return NotFound();

    if (User.IsInRole("Cliente") && !await IsOwner(sale))
      return Forbid();

    var receiptData = BuildReceiptData(sale);
    var pdf = pdfService.GenerateReceipt(receiptData);

    return File(pdf, "application/pdf", $"recibo-{sale.Id}-{sale.SaleDate:yyyyMMdd}.pdf");
  }

  private async Task<bool> IsOwner(Sale sale)
  {
    var userEmail = User.FindFirstValue(ClaimTypes.Email)!;
    var client = await clientService.GetByEmailAsync(userEmail);
    return client?.Id == sale.ClientId;
  }

  private static ReceiptData BuildReceiptData(Sale sale) => new()
  {
    SaleId = sale.Id,
    ClientName = sale.Client.FullName,
    ClientEmail = sale.Client.Email,
    SaleDate = sale.SaleDate,
    Total = sale.Total,
    Items = sale.SaleDetails.Select(sd => new ReceiptItem
    {
      ProductName = sd.Product.Name,
      Quantity = sd.Quantity,
      UnitPrice = sd.UnitPrice
    }).ToList()
  };
}
