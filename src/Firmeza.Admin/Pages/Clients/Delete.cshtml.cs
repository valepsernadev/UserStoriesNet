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
    var client = await clientService.GetByIdAsync(id);
    var name = client?.FullName ?? "Cliente";
    await clientService.DeleteAsync(id);
    TempData["Success"] = $"Cliente \"{name}\" eliminado correctamente.";
    return RedirectToPage("/Clients/Index");
  }
}