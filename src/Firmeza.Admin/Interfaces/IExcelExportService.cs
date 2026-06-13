namespace Firmeza.Admin.Interfaces;

public interface IExcelExportService
{
  Task<byte[]> ExportProductsAsync();
  Task<byte[]> ExportClientsAsync();
  Task<byte[]> ExportSalesAsync();
}
