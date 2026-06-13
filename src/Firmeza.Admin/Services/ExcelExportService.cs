using Firmeza.Admin.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Firmeza.Admin.Services;

public class ExcelExportService(
  IProductService productService,
  IClientService clientService,
  ISaleService saleService) : IExcelExportService
{
  public async Task<byte[]> ExportProductsAsync()
  {
    var products = await productService.GetAllAsync();
    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Productos");

    ws.Cells[1, 1].Value = "ID";
    ws.Cells[1, 2].Value = "Nombre";
    ws.Cells[1, 3].Value = "Descripción";
    ws.Cells[1, 4].Value = "Precio";
    ws.Cells[1, 5].Value = "Stock";
    ws.Cells[1, 6].Value = "Activo";

    ApplyHeaderStyle(ws, 1, 6);

    for (int i = 0; i < products.Count; i++)
    {
      var p = products[i];
      var row = i + 2;
      ws.Cells[row, 1].Value = p.Id;
      ws.Cells[row, 2].Value = p.Name;
      ws.Cells[row, 3].Value = p.Description;
      ws.Cells[row, 4].Value = p.Price;
      ws.Cells[row, 5].Value = p.Stock;
      ws.Cells[row, 6].Value = p.IsActive ? "Sí" : "No";
    }

    ws.Cells.AutoFitColumns();
    return await package.GetAsByteArrayAsync();
  }

  public async Task<byte[]> ExportClientsAsync()
  {
    var clients = await clientService.GetAllAsync();
    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Clientes");

    ws.Cells[1, 1].Value = "ID";
    ws.Cells[1, 2].Value = "Nombre Completo";
    ws.Cells[1, 3].Value = "Documento";
    ws.Cells[1, 4].Value = "Email";
    ws.Cells[1, 5].Value = "Teléfono";

    ApplyHeaderStyle(ws, 1, 5);

    for (int i = 0; i < clients.Count; i++)
    {
      var c = clients[i];
      var row = i + 2;
      ws.Cells[row, 1].Value = c.Id;
      ws.Cells[row, 2].Value = c.FullName;
      ws.Cells[row, 3].Value = c.Document;
      ws.Cells[row, 4].Value = c.Email;
      ws.Cells[row, 5].Value = c.Phone;
    }

    ws.Cells.AutoFitColumns();
    return await package.GetAsByteArrayAsync();
  }

  public async Task<byte[]> ExportSalesAsync()
  {
    var sales = await saleService.GetAllAsync();
    using var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Ventas");

    ws.Cells[1, 1].Value = "ID";
    ws.Cells[1, 2].Value = "Cliente";
    ws.Cells[1, 3].Value = "Email";
    ws.Cells[1, 4].Value = "Fecha";
    ws.Cells[1, 5].Value = "Total";

    ApplyHeaderStyle(ws, 1, 5);

    for (int i = 0; i < sales.Count; i++)
    {
      var s = sales[i];
      var row = i + 2;
      ws.Cells[row, 1].Value = s.Id;
      ws.Cells[row, 2].Value = s.Client.FullName;
      ws.Cells[row, 3].Value = s.Client.Email;
      ws.Cells[row, 4].Value = s.SaleDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
      ws.Cells[row, 5].Value = s.Total;
    }

    ws.Cells.AutoFitColumns();
    return await package.GetAsByteArrayAsync();
  }

  private static void ApplyHeaderStyle(ExcelWorksheet ws, int row, int lastCol)
  {
    using var range = ws.Cells[row, 1, row, lastCol];
    range.Style.Font.Bold = true;
    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 41, 55));
    range.Style.Font.Color.SetColor(Color.White);
  }
}
