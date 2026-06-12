using Microsoft.AspNetCore.Identity;

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
  }
}