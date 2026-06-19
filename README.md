### ¿Qué es?

Firmeza es un ecosistema de software para la gestión y venta de materiales de construcción. Incluye un panel administrativo web, una API REST y un portal de compras para clientes.

### Stack

- **Backend:** .NET 10 / ASP.NET Core / C# 12
- **Base de datos:** PostgreSQL + Entity Framework Core
- **Autenticación:** Identity (cookies para Admin) + JWT (para API y SPA)
- **Frontend Admin:** Razor Pages + Tailwind CSS
- **Frontend Cliente:** React + Vite + Tailwind CSS
- **Reportes:** QuestPDF (PDF) + EPPlus (Excel)
- **Pruebas:** xUnit
- **Despliegue:** Docker + Docker Compose

### Estructura

```
Firmeza/
├── src/
│   ├── Firmeza.Admin/      # Panel administrativo (Razor Pages)
│   ├── Firmeza.API/        # API REST con JWT
│   └── Firmeza.Client/     # SPA React para clientes
├── tests/
│   └── Firmeza.Tests/      # Pruebas unitarias xUnit
├── docker-compose.yml
└── README.md
```

### Ejecución con Docker

Requiere Docker Desktop instalado.

bash

```bash
git clone https://github.com/valepsernadev/UserStoriesNet
cd UserStoriesNet
docker compose up --build
```

El sistema corre las pruebas primero. Si pasan, levanta todos los servicios.

|Servicio|URL|
|---|---|
|Cliente SPA|[http://localhost](http://localhost)|
|Panel Admin|[http://localhost:8080](http://localhost:8080)|
|API REST|[http://localhost:8081](http://localhost:8081)|
|Swagger|[http://localhost:8081/swagger/index.html](http://localhost:8081/swagger/index.html)|

### Ejecución local (desarrollo)

Requiere .NET 10 SDK y PostgreSQL.

bash

```bash
# Terminal 1 — Admin
dotnet run --project src/Firmeza.Admin

# Terminal 2 — API
dotnet run --project src/Firmeza.API

# Terminal 3 — Cliente
cd src/Firmeza.Client
npm install
npm run dev
```

### Credenciales por defecto

|Rol|Email|Contraseña|
|---|---|---|
|Administrador|[admin@firmeza.com](mailto:admin@firmeza.com)|Admin123!|

Los clientes se registran desde el portal en `http://localhost:5173` (dev) o `http://localhost` (Docker).

### Pruebas

bash

```bash
dotnet test
```

### Repositorio

[https://github.com/valepsernadev/UserStoriesNet](https://github.com/valepsernadev/UserStoriesNet)
