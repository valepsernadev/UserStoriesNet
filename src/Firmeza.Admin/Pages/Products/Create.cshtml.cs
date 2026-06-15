using Firmeza.Admin.Models;
using Firmeza.Admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Firmeza.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Admin.Pages.Products;

public class CreateModel(IProductService productService) : PageModel
{
  [BindProperty]
  public ProductInputModel Product { get; set; } = new();

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
      return Page();

    try
    {
      await productService.CreateAsync(new Product
      {
        Name = Product.Name,
        Description = Product.Description,
        Price = Product.Price,
        Stock = Product.Stock
      });
      TempData["Success"] = $"Producto \"{Product.Name}\" creado correctamente.";
      return RedirectToPage("/Products/Index");
    }
    catch (DbUpdateException)
    {
      ModelState.AddModelError(string.Empty, "Error al guardar el producto.");
      return Page();
    }
  }
}

public class ProductInputModel
{
  [Required(ErrorMessage = "El nombre es obligatorio")]
  public string Name { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  [Required(ErrorMessage = "El precio es obligatorio")]
  [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
  public decimal Price { get; set; }

  [Required(ErrorMessage = "El stock es obligatorio")]
  [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
  public int Stock { get; set; }
}