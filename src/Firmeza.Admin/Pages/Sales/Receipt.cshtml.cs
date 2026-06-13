using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Sales;

public class ReceiptModel(ISaleService saleService, IPdfService pdfService) : PageModel
{
  public async Task<IActionResult> OnGetAsync(int id)
  {
    var sale = await saleService.GetByIdAsync(id);

    if (sale is null)
      return NotFound();

    var receiptData = new ReceiptData
    {
      SaleId = sale.Id,
      ClientName = sale.Client.FullName,
      ClientEmail = sale.Client.Email,
      SaleDate = sale.SaleDate,
      Total = sale.Total,
      Items = sale.SaleDetails.Select(sd => new ReceiptItem
      {
        ProductName = sd.Product.Name,
        Quantity = sd.Quantity,
        UnitPrice = sd.UnitPrice
      }).ToList()
    };

    var pdf = pdfService.GenerateReceipt(receiptData);
    return File(pdf, "application/pdf", $"recibo-{sale.Id}.pdf");
  }
}