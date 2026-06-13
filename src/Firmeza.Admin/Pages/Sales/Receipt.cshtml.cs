using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Sales;

public class ReceiptModel(ISaleService saleService, IPdfService pdfService, IWebHostEnvironment env) : PageModel
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

    var fileName = $"recibo-{sale.Id}-{sale.SaleDate:yyyyMMdd}.pdf";
    var folder = Path.Combine(env.WebRootPath, "recibos");
    Directory.CreateDirectory(folder);
    await System.IO.File.WriteAllBytesAsync(Path.Combine(folder, fileName), pdf);

    return File(pdf, "application/pdf", fileName);
  }
}