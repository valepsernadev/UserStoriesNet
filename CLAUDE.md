# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Firmeza** — materiales de construcción. Monolito evolutivo: panel administrativo Razor Pages (`Firmeza.Admin`) que comparte BD con una futura Web API JWT (`Firmeza.API`, HU3).

- **Solución:** `Firmeza.slnx`
- **Runtime:** .NET 10, C# 13
- **BD:** PostgreSQL `localhost:5433`, base `firmeza_db`, usuario `postgres`, contraseña `postgres`
- **Admin por defecto:** `admin@firmeza.com` / `Admin123!`

## Comandos

```bash
# Build completo
dotnet build

# Tests
dotnet test
dotnet test --filter "FullyQualifiedName~NombrePrueba"   # prueba individual

# Migraciones (ejecutar desde la raíz)
dotnet ef migrations add NombreMigracion --project src/Firmeza.Admin --startup-project src/Firmeza.Admin
dotnet ef database update --project src/Firmeza.Admin --startup-project src/Firmeza.Admin

# Correr el panel admin
dotnet run --project src/Firmeza.Admin
```

## Arquitectura

### Proyectos

| Proyecto | Tipo | Puerto |
|---|---|---|
| `src/Firmeza.Admin` | Razor Pages + Identity Cookies | 5001 |
| `tests/Firmeza.Tests` | xUnit, referencia Firmeza.Admin | — |

`Firmeza.API` (HU3) aún no existe; cuando se cree referenciará `Firmeza.Admin` para compartir `ApplicationDbContext` y modelos.

### Capas en Firmeza.Admin

```
Models/          — Entidades EF Core (Product, Client, Sale, SaleDetail, ReceiptData, ReceiptItem, SaleItemInput, ImportResult)
Data/            — ApplicationDbContext (hereda IdentityDbContext), DbSeeder
Interfaces/      — IProductService, IClientService, ISaleService, IExcelImportService, IExcelExportService, IPdfService
Services/        — Implementaciones concretas
Pages/           — Razor PageModels (inyectan interfaces, nunca DbContext directo)
wwwroot/recibos/ — PDFs generados al descargar recibo (creado en tiempo de ejecución)
```

### Flujo de autenticación

- **Panel Razor:** Cookie + rol `Administrador`. Todo `Pages/` está protegido por la política `AdminOnly` excepto `Account/Login`. El rol `Cliente` no puede entrar al panel (ADR-01).
- **API (HU3+):** JWT Bearer + roles `Administrador` / `Cliente`.

### Soft Delete

**Nunca** `db.Remove()`. Todas las entidades tienen `DeletedAt DateTime?`. Todos los queries filtran `DeletedAt == null`. El servicio `DeleteAsync` asigna `DeletedAt = DateTime.UtcNow`.

### Roles Identity

Creados por `DbSeeder` al arrancar: `Administrador`, `Cliente`.

## Convenciones obligatorias

- **File-Scoped Namespaces** en todos los `.cs`
- **Primary Constructors** en servicios y PageModels: `public class ProductService(ApplicationDbContext db) : IProductService`
- Interfaces en `Interfaces/`, implementaciones en `Services/`
- `catch (Exception ex)` solo si `ex` se usa (loguear o relanzar)
- Nunca exponer entidades EF directamente desde la API — usar DTOs

## Razor Pages (Firmeza.Admin)

- **Tailwind CDN** cargado en `_Layout.cshtml`. No hay paso de build CSS.
- Texto mínimo `text-base`. Botones: `py-3 px-6`.
- Errores de formulario: bloque rojo (`bg-red-50 border-red-300 text-red-700`) con lista de errores de `ModelState`.
- Éxito al redirigir: `TempData["Success"] = "mensaje"`, mostrado en Index con bloque verde (`bg-green-50 border-green-300 text-green-700`).

## Paquetes clave y su configuración

| Paquete | Versión | Configuración en Program.cs |
|---|---|---|
| EPPlus | 7.5.3 | `ExcelPackage.LicenseContext = LicenseContext.NonCommercial` |
| QuestPDF | 2026.6.0 | `QuestPDF.Settings.License = LicenseType.Community` |
| Npgsql EF | 10.0.2 | `UseNpgsql(connectionString)` |
| Identity | UI 10.0.9 / EF 9.0.17 | `AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()` |

## Tests

- Framework: xUnit con `Microsoft.EntityFrameworkCore.InMemory`
- Patrón: `DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())` por test para aislamiento
- EPPlus está referenciado en el proyecto de tests para construir streams Excel en memoria
- Tests actuales: `ProductServiceTests` (soft delete), `ExcelImportServiceTests` (validación de filas inválidas)

## Pipeline de HUs

- ✅ **HU1** — Razor Pages + Identity + PostgreSQL + Productos + Clientes + Ventas
- ✅ **HU2** — Carga masiva Excel (EPPlus) + Exportación Excel + Recibos PDF (QuestPDF)
- 🔄 **HU3** — Web API RESTful + JWT + Swagger + SMTP (`Firmeza.API` por crear)
- ⬜ **HU4** — Frontend SPA + Consumo API JWT
- ⬜ **HU5** — Docker Compose CI/CD

## ADRs vigentes

- **ADR-01:** El rol `Cliente` no puede autenticarse en el panel Razor (política `AdminOnly` exige `Administrador`).
- **ADR-02:** Lógica de negocio exclusivamente en servicios (`IXxxService`), no en PageModels, para que HU3 los reutilice.
- **ADR-03:** `ApplicationDbContext` vive en `Firmeza.Admin`; la futura `Firmeza.API` lo referencia via project reference. Se evalúa moverlo a `Firmeza.Core` en HU4+.
