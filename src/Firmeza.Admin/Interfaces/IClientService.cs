using Firmeza.Admin.Models;

namespace Firmeza.Admin.Interfaces;

public interface IClientService
{
  Task<List<Client>> GetAllAsync(string? search = null);
  Task<Client?> GetByIdAsync(int id);
  Task CreateAsync(Client client);
  Task UpdateAsync(Client client);
  Task DeleteAsync(int id);
}