using Firmeza.Admin.Data;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Admin.Services;

public class SaleService (ApplicationDbContext db) : ISaleService
{
  public async Task<List<Sale>> GetAllAsync() =>
    await db.Sales
      .Where(s => s.DeletedAt == null)
      .Include(s => s.Client)
      .Include(s => s.SaleDetails)
      .ThenInclude(sd => sd.Product)
      .OrderByDescending(s => s.SaleDate)
      .ToListAsync();

  public async Task<Sale?> GetByIdAsync(int id) =>
    await db.Sales
      .Include(s => s.Client)
      .Include(s => s.SaleDetails)
      .ThenInclude(sd => sd.Product)
      .FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);

  public async Task<Sale> CreateAsync(int clientId, List<SaleItemInput> items)
  {
    var sale = new Sale
    {
      ClientId = clientId,
      SaleDate = DateTime.UtcNow
    };

    decimal total = 0;

    foreach (var item in items)
    {
      var product = await db.Products.FindAsync(item.ProductId)
                    ?? throw new InvalidOperationException($"Producto {item.ProductId} no encontrado.");

      if (product.Stock < item.Quantity)
        throw new InvalidOperationException($"Stock insuficiente para {product.Name}.");

      product.Stock -= item.Quantity;

      var detail = new SaleDetail
      {
        ProductId = item.ProductId,
        Quantity = item.Quantity,
        UnitPrice = product.Price
      };

      total += detail.UnitPrice * detail.Quantity;
      sale.SaleDetails.Add(detail);
    }

    sale.Total = total;
    db.Sales.Add(sale);
    await db.SaveChangesAsync();

    return sale;
  }
}