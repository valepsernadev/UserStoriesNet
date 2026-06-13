using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Clients;

public class EditModel(IClientService clientService) : PageModel
{
  [BindProperty]
  public Client Client { get; set; } = null!;

  public async Task<IActionResult> OnGetAsync(int id)
  {
    var client = await clientService.GetByIdAsync(id);

    if (client is null)
      return NotFound();

    Client = client;
    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
      return Page();

    await clientService.UpdateAsync(Client);
    TempData["Success"] = $"Cliente \"{Client.FullName}\" actualizado correctamente.";
    return RedirectToPage("/Clients/Index");
  }
}