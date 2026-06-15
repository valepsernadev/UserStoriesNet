namespace Firmeza.Admin.Models;

public class ReceiptItem
{
  public string ProductName { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal Subtotal => Quantity * UnitPrice;
}