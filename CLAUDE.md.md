# Contexto de Desarrollo "Firmeza"

## 1. Estado Actual del Proyecto
- **Historia de Usuario Activa:** HU1 – Módulo Administrativo Base (Infraestructura + Panel + Productos + Clientes)
- **Tarea en Ejecución:** TASK 1 – Creación de la solución vacía y proyecto Razor Pages (.NET 8)
- **Siguiente Hito:** Configuración de dependencias (EF Core, Npgsql, Identity, EPPlus, QuestPDF)

## 2. Stack Tecnológico
- **Arquitectura:** Monolito evolutivo (Razor Pages) → Desacoplado (Web API + JWT + SPA)
- **Backend:** .NET 8 / ASP.NET Core / C# 12
- **Persistencia:** PostgreSQL vía EF Core (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Autenticación:** ASP.NET Core Identity — Cookies para Razor Admin / JWT Bearer para SPA (HU3+)
- **Librerías Core:** EPPlus (Excel), QuestPDF (PDF), xUnit (Pruebas)
- **Despliegue:** Docker / Docker Compose

## 3. Directivas de Estilo y Buenas Prácticas

- **Namespaces:** File-Scoped Namespaces obligatorio en todos los archivos `.cs`
- **Constructores:** Primary Constructors en servicios, repositorios y controladores
- **Nulabilidad:** `<Nullable>enable</Nullable>` activo — todo `null` debe manejarse explícitamente
- **Nombrado:**
  - `PascalCase`: clases, métodos, propiedades, interfaces, ViewModels, DTOs
  - `camelCase`: variables locales, parámetros, campos privados (`_nombre`)
  - Interfaces: prefijo `I` obligatorio (`IProductService`, `IClientRepository`)
- **Errores:** `try-catch` tipificado y específico. `catch (Exception e)` genérico solo si se registra y re-lanza

## 4. Comandos de Referencia

### Migraciones EF Core
```bash
# Crear migración (ejecutar desde la raíz de la solución)
dotnet ef migrations add NombreMigracion \
  --project src/Firmeza.Admin \
  --startup-project src/Firmeza.Admin

# Aplicar migración
dotnet ef database update \
  --project src/Firmeza.Admin \
  --startup-project src/Firmeza.Admin
```

### Pruebas
```bash
dotnet test
```

### Docker
```bash
docker compose up --build
```

## 5. Pipeline de HUs

- [ ] **HU1** — Módulo Administrativo Base (Razor Pages + Identity Cookies + PostgreSQL) ← ACTUAL
- [ ] **HU2** — Carga masiva EPPlus + Exportación + Recibos PDF (QuestPDF)
- [ ] **HU3** — Web API RESTful + JWT + Swagger + SMTP
- [ ] **HU4** — Frontend SPA + Consumo API JWT
- [ ] **HU5** — xUnit + Docker Compose CI/CD

## 6. Registro de Decisiones Arquitectónicas (ADR)

- **ADR-01 — Aislamiento de Identity:** El rol `Cliente` no puede autenticarse en el panel Razor. La política de autorización rechaza cualquier cookie que no tenga el rol `Administrador` y redirige al login con `403`.
- **ADR-02 — Separación de lógica:** La lógica de negocio NO debe residir en los PageModels de Razor. Debe vivir en servicios (`IProductService`, etc.) desde la HU1 para que la HU3 pueda reutilizarla sin reescribir.
- **ADR-03 — DbContext compartido:** El `ApplicationDbContext` se define en `Firmeza.Admin` durante HU1/HU2. En HU3 se evalúa moverlo a un proyecto compartido (`Firmeza.Core` o `Firmeza.Infrastructure`) si la solución lo requiere.

## 7. Reglas de Interacción con el Asistente

- Sin archivos ni gráficas generados a menos que sean solicitados explícitamente
- Crítica técnica inmediata si hay anti-patterns, ineficiencias o violaciones arquitectónicas
- Avance tarea por tarea — no se pasa a la siguiente sin validación de la actual

## 8. Skills Activas para Claude Code

### [Skill: Contexto del Proyecto]
- Solución: `Firmeza.slnx`
- Proyecto principal: `src/Firmeza.Admin`
- Pruebas: `tests/Firmeza.Tests`
- Base de datos: PostgreSQL en localhost:5433, base `firmeza_db`
- Usuario admin por defecto: `admin@firmeza.com` / `Admin123!`

### [Skill: Convenciones Obligatorias]
- File-Scoped Namespaces en todos los `.cs`
- Primary Constructors en servicios y PageModels
- Interfaces en `Interfaces/`, implementaciones en `Services/`
- Modelos en `Models/`, páginas en `Pages/`
- Soft delete obligatorio: nunca borrar registros, usar `DeletedAt`
- Nunca `catch (Exception ex)` sin usar `ex`

### [Skill: Patrones Establecidos]
- Servicios: siempre con interfaz `IXxxService` + implementación `XxxService`
- PageModels: inyectan interfaces, nunca `ApplicationDbContext` directamente
- Validaciones: `ModelState` + mensajes de error visibles en el formulario
- Mensajes de éxito: `TempData["Success"]` al redirigir
- Estilos: Tailwind CDN, texto `text-base` mínimo, botones con `py-3 px-6`

### [Skill: Estado HU2 - En progreso]
- ✅ TASK 1: Carga masiva Excel con EPPlus
- 🔄 TASK 2: Exportación PDF con QuestPDF — licencia pendiente de resolver
- ⬜ TASK 3: Diseño visual
- ⬜ TASK 4: Documentación
- ⬜ TASK 5: Pruebas unitarias
- ⬜ TASK 6: Docker

### [Skill: Problema Activo]
- QuestPDF v2026.6.0 — la configuración de licencia correcta aún no está resuelta
- Mensajes de error en formularios: bloque de validación agregado, pendiente verificar en todos los forms