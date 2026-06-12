using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Admin.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
  : IdentityDbContext(options)
{
  public DbSet<Product> Products => Set<Product>();
  public DbSet<Client> Clients => Set<Client>();
  public DbSet<Sale> Sales => Set<Sale>();
  public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
}