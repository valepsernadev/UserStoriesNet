using Firmeza.Admin.Models;

namespace Firmeza.Admin.Interfaces;

public interface IExcelImportService
{
  Task<ImportResult> ImportAsync(Stream fileStream);
}