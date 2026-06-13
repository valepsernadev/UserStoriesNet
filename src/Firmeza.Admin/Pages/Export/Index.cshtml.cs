using Firmeza.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Export;

public class IndexModel(IExcelExportService excelExportService) : PageModel
{
  public void OnGet() { }

  public async Task<IActionResult> OnGetProductsAsync()
  {
    var bytes = await excelExportService.ExportProductsAsync();
    return File(bytes,
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      $"productos-{DateTime.Now:yyyyMMdd}.xlsx");
  }

  public async Task<IActionResult> OnGetClientsAsync()
  {
    var bytes = await excelExportService.ExportClientsAsync();
    return File(bytes,
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      $"clientes-{DateTime.Now:yyyyMMdd}.xlsx");
  }

  public async Task<IActionResult> OnGetSalesAsync()
  {
    var bytes = await excelExportService.ExportSalesAsync();
    return File(bytes,
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      $"ventas-{DateTime.Now:yyyyMMdd}.xlsx");
  }
}
