using System.ComponentModel.DataAnnotations;

namespace Firmeza.API.DTOs;

public class ClientDto
{
  public int Id { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string Document { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Phone { get; set; } = string.Empty;
}

public class CreateClientDto
{
  [Required(ErrorMessage = "El nombre es obligatorio")]
  public string FullName { get; set; } = string.Empty;

  [Required(ErrorMessage = "El documento es obligatorio")]
  public string Document { get; set; } = string.Empty;

  [Required(ErrorMessage = "El email es obligatorio")]
  [EmailAddress(ErrorMessage = "El email no es válido")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "El teléfono es obligatorio")]
  public string Phone { get; set; } = string.Empty;
}

public class UpdateClientDto
{
  [Required(ErrorMessage = "El nombre es obligatorio")]
  public string FullName { get; set; } = string.Empty;

  [Required(ErrorMessage = "El documento es obligatorio")]
  public string Document { get; set; } = string.Empty;

  [Required(ErrorMessage = "El email es obligatorio")]
  [EmailAddress(ErrorMessage = "El email no es válido")]
  public string Email { get; set; } = string.Empty;

  [Required(ErrorMessage = "El teléfono es obligatorio")]
  public string Phone { get; set; } = string.Empty;
}
