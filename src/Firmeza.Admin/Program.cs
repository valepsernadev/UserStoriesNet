using Firmeza.Admin.Data;
using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Infrastructure;

//Credenciales admin -> admin@firmeza.com / Admin123!

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Protect all pages in the panel
builder.Services.AddRazorPages(options =>
{
  options.Conventions.AuthorizeFolder("/", "AdminOnly");
  options.Conventions.AllowAnonymousToPage("/Account/Login");
});

// Conection to DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Scopes
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<ISaleService, SaleService>();

// Identity configuration 
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
  {
    options.SignIn.RequireConfirmedAccount = false;
  })
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure the Identity redirect
builder.Services.ConfigureApplicationCookie(options =>
{
  options.LoginPath = "/Account/Login";
  options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrador"));
});

// paquetes de importacion de archivos
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
  await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
