using System.Security.Claims;
using AutoMapper;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Firmeza.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientService clientService, IMapper mapper) : ControllerBase
{
  /// <summary>Lista todos los clientes. Solo Administrador.</summary>
  [HttpGet]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> GetAll([FromQuery] string? search)
  {
    var clients = await clientService.GetAllAsync(search);
    return Ok(mapper.Map<List<ClientDto>>(clients));
  }

  /// <summary>Obtiene un cliente por ID. Cliente (propio) o Administrador.</summary>
  [HttpGet("{id:int}")]
  [Authorize]
  public async Task<IActionResult> GetById(int id)
  {
    var client = await clientService.GetByIdAsync(id);

    if (client is null)
      return NotFound();

    if (User.IsInRole("Cliente"))
    {
      var userEmail = User.FindFirstValue(ClaimTypes.Email);
      if (client.Email != userEmail)
        return Forbid();
    }

    return Ok(mapper.Map<ClientDto>(client));
  }

  /// <summary>Crea un cliente. Público.</summary>
  [HttpPost]
  public async Task<IActionResult> Create(CreateClientDto dto)
  {
    var client = mapper.Map<Client>(dto);
    await clientService.CreateAsync(client);
    return CreatedAtAction(nameof(GetById), new { id = client.Id }, mapper.Map<ClientDto>(client));
  }

  /// <summary>Actualiza un cliente. Cliente (propio) o Administrador.</summary>
  [HttpPut("{id:int}")]
  [Authorize]
  public async Task<IActionResult> Update(int id, UpdateClientDto dto)
  {
    var existing = await clientService.GetByIdAsync(id);

    if (existing is null)
      return NotFound();

    if (User.IsInRole("Cliente"))
    {
      var userEmail = User.FindFirstValue(ClaimTypes.Email);
      if (existing.Email != userEmail)
        return Forbid();
    }

    mapper.Map(dto, existing);
    await clientService.UpdateAsync(existing);
    return Ok(mapper.Map<ClientDto>(existing));
  }

  /// <summary>Elimina (soft delete) un cliente. Solo Administrador.</summary>
  [HttpDelete("{id:int}")]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> Delete(int id)
  {
    var existing = await clientService.GetByIdAsync(id);

    if (existing is null)
      return NotFound();

    await clientService.DeleteAsync(id);
    return NoContent();
  }
}
