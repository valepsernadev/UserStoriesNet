using Firmeza.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Admin.Data;

public class DbSeeder
{
  public static async Task SeedAsync(IServiceProvider serviceProvider)
  {
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Administrador"))
      await roleManager.CreateAsync(new IdentityRole("Administrador"));

    if (!await roleManager.RoleExistsAsync("Cliente"))
      await roleManager.CreateAsync(new IdentityRole("Cliente"));

    const string adminEmail = "admin@firmeza.com";
    const string adminPassword = "Admin123!";

    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
      var admin = new IdentityUser
      {
        UserName = adminEmail,
        Email = adminEmail,
        EmailConfirmed = true
      };

      var result = await userManager.CreateAsync(admin, adminPassword);

      if (result.Succeeded)
        await userManager.AddToRoleAsync(admin, "Administrador");
    }

    var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

    if (!await db.Products.AnyAsync())
    {
      db.Products.AddRange(
        new Product { Name = "Cemento gris 50kg", Description = "Cemento portland gris para construcción general", Price = 28000, Stock = 200, IsActive = true },
        new Product { Name = "Arena de río x bulto", Description = "Arena lavada de río para mezclas y concretos", Price = 8000, Stock = 500, IsActive = true },
        new Product { Name = "Ladrillo tolete x1000", Description = "Ladrillo tolete estándar para mampostería", Price = 650000, Stock = 50, IsActive = true },
        new Product { Name = "Varilla 1/2 x 6m", Description = "Varilla corrugada de 1/2 pulgada para refuerzo estructural", Price = 45000, Stock = 150, IsActive = true },
        new Product { Name = "Bloque No.4 x100", Description = "Bloque de concreto No.4 para muros y divisiones", Price = 85000, Stock = 80, IsActive = true },
        new Product { Name = "Tubería PVC 4 pulgadas", Description = "Tubo PVC sanitario de 4 pulgadas para desagüe", Price = 32000, Stock = 100, IsActive = true },
        new Product { Name = "Pintura blanca 1 galón", Description = "Pintura vinílica blanca para interiores y exteriores", Price = 52000, Stock = 75, IsActive = true },
        new Product { Name = "Impermeabilizante x galón", Description = "Impermeabilizante acrílico para techos y fachadas", Price = 68000, Stock = 60, IsActive = true }
      );

      await db.SaveChangesAsync();
    }
  }
}