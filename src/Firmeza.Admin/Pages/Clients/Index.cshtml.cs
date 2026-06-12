using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Clients;

public class IndexModel(IClientService clientService) : PageModel
{
  public List<Client> Clients { get; set; } = [];
  public string? Search { get; set; }

  public async Task OnGetAsync(string? search)
  {
    Search = search;
    Clients = await clientService.GetAllAsync(search);
  }
}