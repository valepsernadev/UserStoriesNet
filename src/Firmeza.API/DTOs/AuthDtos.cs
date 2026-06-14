using System.ComponentModel.DataAnnotations;

namespace Firmeza.API.DTOs;

public class RegisterDto
{
  [Required(ErrorMessage = "El email es obligatorio")]
  [EmailAddress(ErrorMessage = "El email no es válido")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "La contraseña es obligatoria")]
  [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
  public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
  [Required(ErrorMessage = "El email es obligatorio")]
  [EmailAddress(ErrorMessage = "El email no es válido")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "La contraseña es obligatoria")]
  public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
  public string Token { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public DateTime Expiration { get; set; }
}
