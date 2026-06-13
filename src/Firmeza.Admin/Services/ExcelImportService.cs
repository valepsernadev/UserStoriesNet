using Firmeza.Admin.Data;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Firmeza.Admin.Services;

public class ExcelImportService (ApplicationDbContext db) : IExcelImportService
{
  public async Task<ImportResult> ImportAsync(Stream fileStream)
    {
        var result = new ImportResult();

        using var package = new ExcelPackage(fileStream);

        foreach (var worksheet in package.Workbook.Worksheets)
        {
            var headers = GetHeaders(worksheet);

            if (IsProductSheet(headers))
                await ImportProducts(worksheet, headers, result);
            else if (IsClientSheet(headers))
                await ImportClients(worksheet, headers, result);
        }

        return result;
    }

    private static Dictionary<string, int> GetHeaders(ExcelWorksheet sheet)
    {
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (sheet.Dimension is null) return headers;

        for (int col = 1; col <= sheet.Dimension.Columns; col++)
        {
            var header = sheet.Cells[1, col].Text.Trim();
            if (!string.IsNullOrEmpty(header))
                headers[header] = col;
        }

        return headers;
    }

    private static bool IsProductSheet(Dictionary<string, int> headers) =>
        headers.ContainsKey("nombre") || headers.ContainsKey("precio") || headers.ContainsKey("stock");

    private static bool IsClientSheet(Dictionary<string, int> headers) =>
        headers.ContainsKey("documento") || headers.ContainsKey("email") || headers.ContainsKey("telefono");

    private async Task ImportProducts(ExcelWorksheet sheet, Dictionary<string, int> headers, ImportResult result)
    {
        if (sheet.Dimension is null) return;

        for (int row = 2; row <= sheet.Dimension.Rows; row++)
        {
            try
            {
                var name = GetCell(sheet, row, headers, "nombre");
                var priceText = GetCell(sheet, row, headers, "precio");
                var stockText = GetCell(sheet, row, headers, "stock");

                if (string.IsNullOrWhiteSpace(name))
                {
                    result.Errors.Add($"Fila {row}: nombre de producto vacío.");
                    continue;
                }

                if (!decimal.TryParse(priceText, out var price) || price <= 0)
                {
                    result.Errors.Add($"Fila {row}: precio inválido '{priceText}'.");
                    continue;
                }

                if (!int.TryParse(stockText, out var stock) || stock < 0)
                {
                    result.Errors.Add($"Fila {row}: stock inválido '{stockText}'.");
                    continue;
                }

                var existing = await db.Products
                    .FirstOrDefaultAsync(p => p.Name == name && p.DeletedAt == null);

                if (existing is not null)
                {
                    existing.Price = price;
                    existing.Stock = stock;
                }
                else
                {
                    db.Products.Add(new Product
                    {
                        Name = name,
                        Description = GetCell(sheet, row, headers, "descripcion"),
                        Price = price,
                        Stock = stock
                    });
                }

                result.ProductsImported++;
            }
            catch (Exception)
            {
                result.Errors.Add($"Fila {row}: error inesperado al procesar producto.");
            }
        }

        await db.SaveChangesAsync();
    }

    private async Task ImportClients(ExcelWorksheet sheet, Dictionary<string, int> headers, ImportResult result)
    {
        if (sheet.Dimension is null) return;

        for (int row = 2; row <= sheet.Dimension.Rows; row++)
        {
            try
            {
                var fullName = GetCell(sheet, row, headers, "nombre");
                var document = GetCell(sheet, row, headers, "documento");
                var email = GetCell(sheet, row, headers, "email");
                var phone = GetCell(sheet, row, headers, "telefono");

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(document))
                {
                    result.Errors.Add($"Fila {row}: nombre o documento vacío.");
                    continue;
                }

                var existing = await db.Clients
                    .FirstOrDefaultAsync(c => c.Document == document && c.DeletedAt == null);

                if (existing is not null)
                {
                    existing.FullName = fullName;
                    existing.Email = email;
                    existing.Phone = phone;
                }
                else
                {
                    db.Clients.Add(new Client
                    {
                        FullName = fullName,
                        Document = document,
                        Email = email,
                        Phone = phone
                    });
                }

                result.ClientsImported++;
            }
            catch (Exception)
            {
                result.Errors.Add($"Fila {row}: error inesperado al procesar cliente.");
            }
        }

        await db.SaveChangesAsync();
    }

    private static string GetCell(ExcelWorksheet sheet, int row, Dictionary<string, int> headers, string key) =>
        headers.TryGetValue(key, out var col) ? sheet.Cells[row, col].Text.Trim() : string.Empty;
}