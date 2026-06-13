using Firmeza.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Firmeza.Admin.Models;

namespace Firmeza.Admin.Pages.Sales;

public class CreateModel(ISaleService saleService, IClientService clientService, IProductService productService) : PageModel
{
  [BindProperty]
  [Required(ErrorMessage = "Selecciona un cliente")]
  public int ClientId { get; set; }

  [BindProperty]
  public List<SaleItemInput> Items { get; set; } = [];

  public List<SelectListItem> ClientOptions { get; set; } = [];
  public List<SelectListItem> ProductOptions { get; set; } = [];

  public async Task OnGetAsync()
  {
    await LoadOptionsAsync();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    var validItems = Items.Where(i => i.ProductId > 0 && i.Quantity > 0).ToList();

    if (validItems.Count == 0)
    {
      ModelState.AddModelError(string.Empty, "Agrega al menos un producto.");
      await LoadOptionsAsync();
      return Page();
    }

    try
    {
      await saleService.CreateAsync(ClientId, validItems);
      TempData["Success"] = "Venta registrada correctamente.";
      return RedirectToPage("/Sales/Index");
    }
    catch (InvalidOperationException ex)
    {
      ModelState.AddModelError(string.Empty, ex.Message);
      await LoadOptionsAsync();
      return Page();
    }
  }

  private async Task LoadOptionsAsync()
  {
    var clients = await clientService.GetAllAsync();
    ClientOptions = clients.Select(c => new SelectListItem(c.FullName, c.Id.ToString())).ToList();

    var products = await productService.GetAllAsync();
    ProductOptions = products.Select(p => new SelectListItem($"{p.Name} — ${p.Price:N2} (Stock: {p.Stock})", p.Id.ToString())).ToList();
  }
}