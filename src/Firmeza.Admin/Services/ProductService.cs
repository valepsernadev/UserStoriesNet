using Firmeza.Admin.Data;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Admin.Services;

public class ProductService(ApplicationDbContext db) : IProductService
{
  public async Task<List<Product>> GetAllAsync(string? search = null)
  {
    var query = db.Products
      .Where(p => p.DeletedAt == null)
      .AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
      query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

    return await query.OrderBy(p => p.Name).ToListAsync();
  }

  public async Task<Product?> GetByIdAsync(int id) =>
    await db.Products.FindAsync(id);

  public async Task CreateAsync(Product product)
  {
    db.Products.Add(product);
    await db.SaveChangesAsync();
  }

  public async Task UpdateAsync(Product product)
  {
    db.Products.Update(product);
    await db.SaveChangesAsync();
  }

  public async Task DeleteAsync(int id)
  {
    var product = await db.Products.FindAsync(id);
    if (product is not null)
    {
      product.DeletedAt = DateTime.UtcNow;
      await db.SaveChangesAsync();
    }
  }
}