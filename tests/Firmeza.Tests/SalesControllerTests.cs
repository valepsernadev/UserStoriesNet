using System.Security.Claims;
using AutoMapper;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Firmeza.API.Controllers;
using Firmeza.API.DTOs;
using Firmeza.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Firmeza.Tests;

public class SalesControllerTests
{
  private static SalesController BuildController(
    Mock<ISaleService> saleService,
    Mock<IMapper> mapper,
    Mock<IClientService> clientService,
    Mock<IPdfService> pdfService,
    Mock<IEmailService> emailService,
    string role = "Administrador")
  {
    var controller = new SalesController(
      saleService.Object,
      mapper.Object,
      clientService.Object,
      pdfService.Object,
      emailService.Object,
      NullLogger<SalesController>.Instance);

    controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = new ClaimsPrincipal(new ClaimsIdentity(
        [
          new Claim(ClaimTypes.Role, role),
          new Claim(ClaimTypes.Email, "admin@firmeza.com")
        ], "Test"))
      }
    };

    return controller;
  }

  [Fact]
  public async Task GetAll_AsAdministrador_ReturnsOkWithSaleList()
  {
    var saleService = new Mock<ISaleService>();
    var mapper = new Mock<IMapper>();
    var clientService = new Mock<IClientService>();
    var pdfService = new Mock<IPdfService>();
    var emailService = new Mock<IEmailService>();

    var sales = new List<Sale>
    {
      new() { Id = 1, ClientId = 1, Total = 50000, SaleDate = DateTime.UtcNow }
    };
    var dtos = new List<SaleDto>
    {
      new() { Id = 1, ClientId = 1, Total = 50000 }
    };

    saleService.Setup(s => s.GetAllAsync()).ReturnsAsync(sales);
    mapper.Setup(m => m.Map<List<SaleDto>>(sales)).Returns(dtos);

    var controller = BuildController(saleService, mapper, clientService, pdfService, emailService);

    var result = await controller.GetAll();

    var ok = Assert.IsType<OkObjectResult>(result);
    var returned = Assert.IsType<List<SaleDto>>(ok.Value);
    Assert.Single(returned);
    Assert.Equal(1, returned[0].Id);
  }

  [Fact]
  public async Task GetById_WhenSaleNotFound_ReturnsNotFound()
  {
    var saleService = new Mock<ISaleService>();
    var mapper = new Mock<IMapper>();
    var clientService = new Mock<IClientService>();
    var pdfService = new Mock<IPdfService>();
    var emailService = new Mock<IEmailService>();

    saleService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Sale?)null);

    var controller = BuildController(saleService, mapper, clientService, pdfService, emailService);

    var result = await controller.GetById(99);

    Assert.IsType<NotFoundResult>(result);
  }

  [Fact]
  public async Task Create_AsAdministrador_WithMissingClientId_ReturnsBadRequest()
  {
    var saleService = new Mock<ISaleService>();
    var mapper = new Mock<IMapper>();
    var clientService = new Mock<IClientService>();
    var pdfService = new Mock<IPdfService>();
    var emailService = new Mock<IEmailService>();

    var controller = BuildController(saleService, mapper, clientService, pdfService, emailService);

    var dto = new CreateSaleDto
    {
      ClientId = null,
      Items = [new SaleItemDto { ProductId = 1, Quantity = 2 }]
    };

    var result = await controller.Create(dto);

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task Create_WhenStockInsuficiente_ReturnsBadRequest()
  {
    var saleService = new Mock<ISaleService>();
    var mapper = new Mock<IMapper>();
    var clientService = new Mock<IClientService>();
    var pdfService = new Mock<IPdfService>();
    var emailService = new Mock<IEmailService>();

    var client = new Client { Id = 1, FullName = "Juan Pérez", Email = "juan@test.com", Document = "123" };
    clientService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(client);
    saleService
      .Setup(s => s.CreateAsync(1, It.IsAny<List<SaleItemInput>>()))
      .ThrowsAsync(new InvalidOperationException("Stock insuficiente para Cemento."));

    var controller = BuildController(saleService, mapper, clientService, pdfService, emailService);

    var dto = new CreateSaleDto
    {
      ClientId = 1,
      Items = [new SaleItemDto { ProductId = 1, Quantity = 9999 }]
    };

    var result = await controller.Create(dto);

    Assert.IsType<BadRequestObjectResult>(result);
  }
}
