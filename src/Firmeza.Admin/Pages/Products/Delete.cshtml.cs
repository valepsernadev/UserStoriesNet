using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Firmeza.Admin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Products;

public class DeleteModel(IProductService productService) : PageModel
{
  public Product? Product { get; set; }

  public async Task<IActionResult> OnGetAsync(int id)
  {
    Product = await productService.GetByIdAsync(id);

    if (Product is null)
      return NotFound();

    return Page();
  }

  public async Task<IActionResult> OnPostAsync(int id)
  {
    await productService.DeleteAsync(id);
    return RedirectToPage("/Products/Index");
  }
}