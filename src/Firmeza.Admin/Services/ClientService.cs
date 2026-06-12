using Firmeza.Admin.Data;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Admin.Services;

public class ClientService(ApplicationDbContext db) : IClientService
{
  public async Task<List<Client>> GetAllAsync(string? search = null)
  {
    var query = db.Clients
      .Where(c => c.DeletedAt == null)
      .AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
      query = query.Where(c => c.FullName.Contains(search) || c.Document.Contains(search));

    return await query.OrderBy(c => c.FullName).ToListAsync();
  }

  public async Task<Client?> GetByIdAsync(int id) =>
    await db.Clients.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

  public async Task CreateAsync(Client client)
  {
    db.Clients.Add(client);
    await db.SaveChangesAsync();
  }

  public async Task UpdateAsync(Client client)
  {
    db.Clients.Update(client);
    await db.SaveChangesAsync();
  }

  public async Task DeleteAsync(int id)
  {
    var client = await db.Clients.FindAsync(id);
    if (client is not null)
    {
      client.DeletedAt = DateTime.UtcNow;
      await db.SaveChangesAsync();
    }
  }
}