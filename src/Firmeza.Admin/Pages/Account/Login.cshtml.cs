using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Firmeza.Admin.Pages.Account;

public class LoginModel(SignInManager<IdentityUser> signInManager) : PageModel
{
  [BindProperty]
  [Required(ErrorMessage = "El email es obligatorio")]
  [EmailAddress(ErrorMessage = "Email no válido")]
  public string Email { get; set; } = string.Empty;

  [BindProperty]
  [Required(ErrorMessage = "La contraseña es obligatoria")]
  public string Password { get; set; } = string.Empty;

  public string? ErrorMessage { get; set; }

  public async Task OnGetAsync()
  {
    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
      return Page();

    try
    {
      var result = await signInManager.PasswordSignInAsync(Email, Password, isPersistent: false, lockoutOnFailure: false);

      if (result.Succeeded)
        return RedirectToPage("/Index");

      ErrorMessage = "Credenciales incorrectas. Verifica tu email y contraseña.";
      return Page();
    }
    catch (Exception ex)
    {
      ErrorMessage = "Ocurrió un error al iniciar sesión. Intenta nuevamente.";
      return Page();
    }
  }
}