using Firmeza.Admin.Models;

namespace Firmeza.Admin.Interfaces;

public interface ISaleService
{
  Task<List<Sale>> GetAllAsync();
  Task<Sale?> GetByIdAsync(int id);
  Task<Sale> CreateAsync(int clientId, List<SaleItemInput> items);
}