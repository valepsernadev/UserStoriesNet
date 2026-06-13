using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Sales;

public class IndexModel(ISaleService saleService) : PageModel
{
  public List<Sale> Sales { get; set; } = [];

  public async Task OnGetAsync()
  {
    Sales = await saleService.GetAllAsync();
  }
}