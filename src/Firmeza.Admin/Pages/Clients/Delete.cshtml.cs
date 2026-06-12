using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Clients;

public class DeleteModel(IClientService clientService) : PageModel
{
  public Client? Client { get; set; }

  public async Task<IActionResult> OnGetAsync(int id)
  {
    Client = await clientService.GetByIdAsync(id);

    if (Client is null)
      return NotFound();

    return Page();
  }

  public async Task<IActionResult> OnPostAsync(int id)
  {
    await clientService.DeleteAsync(id);
    return RedirectToPage("/Clients/Index");
  }
}