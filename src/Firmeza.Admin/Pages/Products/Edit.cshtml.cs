using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Firmeza.Admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Products;

public class EditModel(IProductService productService) : PageModel
{
  [BindProperty]
  public Product Product { get; set; } = null!;

  public async Task<IActionResult> OnGetAsync(int id)
  {
    var product = await productService.GetByIdAsync(id);

    if (product is null)
      return NotFound();

    Product = product;
    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
      return Page();

    await productService.UpdateAsync(Product);
    TempData["Success"] = $"Producto \"{Product.Name}\" actualizado correctamente.";
    return RedirectToPage("/Products/Index");
  }
}