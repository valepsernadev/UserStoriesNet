using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Firmeza.Admin.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Products;

public class IndexModel(IProductService productService) : PageModel
{
  public List<Product> Products { get; set; } = [];
  public string? Search { get; set; }

  public async Task OnGetAsync(string? search)
  {
    Search = search;
    Products = await productService.GetAllAsync(search);
  }
}