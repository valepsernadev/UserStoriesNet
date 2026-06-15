using Firmeza.Admin.Data;
using Firmeza.Admin.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Firmeza.Tests;

public class ExcelImportServiceTests
{
  private static ApplicationDbContext CreateInMemoryContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;
    return new ApplicationDbContext(options);
  }

  private static Stream BuildProductSheet(params (string nombre, string precio, string stock)[] rows)
  {
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    var package = new ExcelPackage();
    var ws = package.Workbook.Worksheets.Add("Hoja1");
    ws.Cells[1, 1].Value = "nombre";
    ws.Cells[1, 2].Value = "precio";
    ws.Cells[1, 3].Value = "stock";
    ws.Cells[1, 4].Value = "descripcion";

    for (int i = 0; i < rows.Length; i++)
    {
      ws.Cells[i + 2, 1].Value = rows[i].nombre;
      ws.Cells[i + 2, 2].Value = rows[i].precio;
      ws.Cells[i + 2, 3].Value = rows[i].stock;
    }

    var ms = new MemoryStream();
    package.SaveAs(ms);
    ms.Position = 0;
    return ms;
  }

  [Fact]
  public async Task ImportAsync_RowWithEmptyName_AddsErrorAndDoesNotInsert()
  {
    var context = CreateInMemoryContext();
    var service = new ExcelImportService(context);

    var stream = BuildProductSheet(("", "100", "5"));

    var result = await service.ImportAsync(stream);

    Assert.Contains(result.Errors, e => e.Contains("nombre"));
    Assert.Equal(0, result.ProductsImported);
    Assert.Empty(await context.Products.ToListAsync());
  }

  [Fact]
  public async Task ImportAsync_RowWithInvalidPrice_AddsErrorAndDoesNotInsert()
  {
    var context = CreateInMemoryContext();
    var service = new ExcelImportService(context);

    var stream = BuildProductSheet(("Ladrillo", "no-es-precio", "10"));

    var result = await service.ImportAsync(stream);

    Assert.Contains(result.Errors, e => e.Contains("precio"));
    Assert.Equal(0, result.ProductsImported);
    Assert.Empty(await context.Products.ToListAsync());
  }

  [Fact]
  public async Task ImportAsync_RowWithNegativeStock_AddsErrorAndDoesNotInsert()
  {
    var context = CreateInMemoryContext();
    var service = new ExcelImportService(context);

    var stream = BuildProductSheet(("Arena", "50", "-1"));

    var result = await service.ImportAsync(stream);

    Assert.Contains(result.Errors, e => e.Contains("stock"));
    Assert.Equal(0, result.ProductsImported);
    Assert.Empty(await context.Products.ToListAsync());
  }

  [Fact]
  public async Task ImportAsync_MultipleInvalidRows_AllGenerateErrorsNoneInserted()
  {
    var context = CreateInMemoryContext();
    var service = new ExcelImportService(context);

    var stream = BuildProductSheet(
      ("", "100", "5"),
      ("Ladrillo", "abc", "10"),
      ("Arena", "50", "-1")
    );

    var result = await service.ImportAsync(stream);

    Assert.Equal(3, result.Errors.Count);
    Assert.Equal(0, result.ProductsImported);
    Assert.Empty(await context.Products.ToListAsync());
  }
}
