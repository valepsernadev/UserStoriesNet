using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Firmeza.Admin.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
  : IdentityDbContext(options)
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.ConfigureWarnings(w => 
      w.Ignore(RelationalEventId.PendingModelChangesWarning));
  }
  
  public DbSet<Product> Products => Set<Product>();
  public DbSet<Client> Clients => Set<Client>();
  public DbSet<Sale> Sales => Set<Sale>();
  public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
}