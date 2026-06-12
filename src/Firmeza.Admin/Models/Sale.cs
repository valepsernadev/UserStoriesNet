namespace Firmeza.Admin.Models;

public class Sale
{
  public int Id { get; set; }
  public int ClientId { get; set; }
  public Client Client { get; set; } = null!;
  public DateTime SaleDate { get; set; } = DateTime.UtcNow;
  public decimal Total { get; set; }
  public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
  public DateTime? DeletedAt { get; set; }
}