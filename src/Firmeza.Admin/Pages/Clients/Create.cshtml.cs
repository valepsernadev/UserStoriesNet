using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Firmeza.Admin.Pages.Clients;

public class CreateModel(IClientService clientService) : PageModel
{
  [BindProperty]
  public ClientInputModel Client { get; set; } = new();

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
      return Page();

    await clientService.CreateAsync(new Client
    {
      FullName = Client.FullName,
      Document = Client.Document,
      Email = Client.Email,
      Phone = Client.Phone
    });

    return RedirectToPage("/Clients/Index");
  }
}

public class ClientInputModel
{
  [Required(ErrorMessage = "El nombre es obligatorio")]
  public string FullName { get; set; } = string.Empty;

  [Required(ErrorMessage = "El documento es obligatorio")]
  public string Document { get; set; } = string.Empty;

  [Required(ErrorMessage = "El email es obligatorio")]
  [EmailAddress(ErrorMessage = "El email no es válido")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "El teléfono es obligatorio")]
  [Phone(ErrorMessage = "El teléfono no es válido")]
  public string Phone { get; set; } = string.Empty;
}