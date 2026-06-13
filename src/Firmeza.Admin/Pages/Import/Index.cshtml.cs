using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Firmeza.Admin.Pages.Import;

public class IndexModel(IExcelImportService importService) : PageModel
{
  public ImportResult? Result { get; set; }

  public async Task<IActionResult> OnPostAsync(IFormFile file)
  {
    if (file is null || file.Length == 0)
    {
      ModelState.AddModelError(string.Empty, "Selecciona un archivo válido.");
      return Page();
    }

    if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
    {
      ModelState.AddModelError(string.Empty, "Solo se permiten archivos .xlsx");
      return Page();
    }

    try
    {
      using var stream = file.OpenReadStream();
      Result = await importService.ImportAsync(stream);
    }
    catch (Exception)
    {
      ModelState.AddModelError(string.Empty, "Error al procesar el archivo.");
    }

    return Page();
  }
}