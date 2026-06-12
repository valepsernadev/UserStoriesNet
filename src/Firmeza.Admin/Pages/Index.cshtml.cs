using Firmeza.Admin.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages;

public class IndexModel(ApplicationDbContext db) : PageModel
{
  public int TotalProducts { get; set; }
  public int TotalClients { get; set; }
  public int TotalSales { get; set; }

  public void OnGet()
  {
    TotalProducts = db.Products.Count(p => p.DeletedAt == null);
    TotalClients = db.Clients.Count(c => c.DeletedAt == null);
    TotalSales = db.Sales.Count(s => s.DeletedAt == null);
  }
}