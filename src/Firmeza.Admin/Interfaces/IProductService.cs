using Firmeza.Admin.Models;

namespace Firmeza.Admin.Interfaces;

public interface IProductService
{
  Task<List<Product>> GetAllAsync(string? search = null);
  Task<Product?> GetByIdAsync(int id);
  Task CreateAsync(Product product);
  Task UpdateAsync(Product product);
  Task DeleteAsync(int id);
}