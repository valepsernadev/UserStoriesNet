using System.ComponentModel.DataAnnotations;

namespace Firmeza.API.DTOs;

public class SaleDto
{
  public int Id { get; set; }
  public int ClientId { get; set; }
  public string ClientName { get; set; } = string.Empty;
  public DateTime SaleDate { get; set; }
  public decimal Total { get; set; }
  public List<SaleDetailDto> Items { get; set; } = [];
}

public class SaleDetailDto
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal Subtotal { get; set; }
}

public class CreateSaleDto
{
  public int? ClientId { get; set; }

  [Required(ErrorMessage = "Debes agregar al menos un producto")]
  [MinLength(1, ErrorMessage = "Debes agregar al menos un producto")]
  public List<SaleItemDto> Items { get; set; } = [];
}

public class SaleItemDto
{
  [Range(1, int.MaxValue, ErrorMessage = "ProductId inválido")]
  public int ProductId { get; set; }

  [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
  public int Quantity { get; set; }
}
