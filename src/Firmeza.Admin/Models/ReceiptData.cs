namespace Firmeza.Admin.Models;

public class ReceiptData
{
  public int SaleId { get; set; }
  public string ClientName { get; set; } = string.Empty;
  public string ClientEmail { get; set; } = string.Empty;
  public DateTime SaleDate { get; set; }
  public List<ReceiptItem> Items { get; set; } = [];
  public decimal Total { get; set; }
}