using AutoMapper;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Firmeza.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService, IMapper mapper) : ControllerBase
{
  /// <summary>Lista todos los productos activos. Público.</summary>
  [HttpGet]
  public async Task<IActionResult> GetAll([FromQuery] string? search)
  {
    var products = await productService.GetAllAsync(search);
    return Ok(mapper.Map<List<ProductDto>>(products));
  }

  /// <summary>Obtiene un producto por ID. Público.</summary>
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    var product = await productService.GetByIdAsync(id);

    if (product is null || product.DeletedAt.HasValue)
      return NotFound();

    return Ok(mapper.Map<ProductDto>(product));
  }

  /// <summary>Crea un producto. Solo Administrador.</summary>
  [HttpPost]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> Create(CreateProductDto dto)
  {
    var product = mapper.Map<Product>(dto);
    await productService.CreateAsync(product);
    return CreatedAtAction(nameof(GetById), new { id = product.Id }, mapper.Map<ProductDto>(product));
  }

  /// <summary>Actualiza un producto. Solo Administrador.</summary>
  [HttpPut("{id:int}")]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> Update(int id, UpdateProductDto dto)
  {
    var existing = await productService.GetByIdAsync(id);

    if (existing is null || existing.DeletedAt.HasValue)
      return NotFound();

    mapper.Map(dto, existing);
    await productService.UpdateAsync(existing);
    return Ok(mapper.Map<ProductDto>(existing));
  }

  /// <summary>Elimina (soft delete) un producto. Solo Administrador.</summary>
  [HttpDelete("{id:int}")]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> Delete(int id)
  {
    var existing = await productService.GetByIdAsync(id);

    if (existing is null || existing.DeletedAt.HasValue)
      return NotFound();

    await productService.DeleteAsync(id);
    return NoContent();
  }
}
