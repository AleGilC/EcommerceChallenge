# EcommerceChallenge – .NET 8 Microservices (Products + Orders)

> **Stack**: .NET 8 · ASP.NET Core Web API · EF Core · Clean Architecture por servicio · FluentValidation · xUnit + Moq + FluentAssertions · EFCore InMemory para repos · Docker & docker‑compose · Health Checks · IHttpClientFactory + Polly · Swagger/OpenAPI

---

## ✅ Instrucciones de ejecución

### 1) Requisitos
- **.NET SDK 8.0+**
- **Docker** y **Docker Compose**
- (Opcional) **dotnet-ef** para crear/aplicar migraciones manualmente:  
  `dotnet tool install --global dotnet-ef`

### 2) Estructura de carpetas (resumen)
```
EcommerceChallenge/
├─ services/
│  ├─ ProductService/
│  │  ├─ ProductService.API/
│  │  ├─ ProductService.Application/
│  │  ├─ ProductService.Domain/
│  │  └─ ProductService.Infrastructure/
│  └─ OrderService/
│     ├─ OrderService.API/
│     ├─ OrderService.Application/
│     ├─ OrderService.Domain/
│     └─ OrderService.Infrastructure/
├─ tests/
│  ├─ ProductService.Tests/
│  └─ OrderService.Tests/
├─ docker-compose.yml
└─ README.md  ← este archivo
```

### 3) Variables importantes
- **ConnString (SQL Server en Docker):**  
  `Server=mssql;Database=ProductsDb|OrdersDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True`
- **URL ProductService (desde OrderService):** `http://productservice:8080`

Estas variables ya están parametrizadas en `docker-compose.yml` mediante `environment`.

### 4) Levantar todo con Docker
```bash
docker compose up --build
```
- ProductService → **http://localhost:8080/swagger**
- OrderService   → **http://localhost:8081/swagger**
- Health checks  → `/health` en ambos

> El arranque aplica **migraciones** automáticamente y si la tabla de productos está vacía, hace un **seed** mínimo.

### 5) Ejecutar local sin Docker (opcional)
- Ajusta `ConnectionStrings:Default` en cada `appsettings.json` a tu SQL Server local.
- Desde cada API:
  ```bash
  dotnet ef database update
  dotnet run
  ```
- Recomendado cambiar `ASPNETCORE_URLS` o `launchSettings.json` para puertos definidos.

### 6) Tests y cobertura (≥ 80%)
```bash
# ejecutar en la raíz del repo
dotnet test   /p:CollectCoverage=true   /p:CoverletOutputFormat=cobertura   /p:Threshold=80   /p:ThresholdType=line
```
El proyecto incluye tests de **servicios**, **repositorios** (EF InMemory) y **controllers** con **Moq** y **FluentAssertions**.

---

## 🧭 Arquitectura del sistema

### Microservicios
- **ProductService**  
  - CRUD de productos y paginación.  
  - Modelo: `Product (Id, Name, Description, Price, Stock, CreatedAt, UpdatedAt)`  
  - Capas: `Domain`, `Application` (DTOs, Services, Validators), `Infrastructure` (EF + Repos), `API` (Controllers, Program).
  - DB: `ProductsDb` (SQL Server en docker).

- **OrderService**  
  - Pedidos con ítems, estado y total calculado.  
  - Modelo: `Order (Id, CustomerId, Status, TotalAmount, OrderDate, Items)`; `OrderItem (ProductId, Quantity, UnitPrice)`; `OrderStatus (enum)`  
  - **Comunicación** con ProductService vía `IHttpClientFactory` + **Polly** (retries transitorios).  
  - Validación de existencia de producto y stock suficiente antes de crear pedido.  
  - DB: `OrdersDb` (SQL Server en docker).

### Patrones y prácticas
- **Clean Architecture por servicio**: separación clara por capas.
- **Repository Pattern**: interfaces + implementación EF Core.
- **FluentValidation**: validación explícita en endpoints/servicios (sin paquete AspNetCore deprecado).
- **Manejo de errores**: `UseExceptionHandler("/error")` → `ProblemDetails` genérico.
- **Health Checks**: `/health` en ambos servicios.
- **OpenAPI/Swagger**: documentación y prueba de endpoints.
- **Code First + Migrations**: `db.Database.Migrate()` en arranque para ambientes dev/demo.

Diagrama lógico (alto nivel):

```
   +--------------------+            HTTP + Polly            +-------------------+
   |  OrderService.API  |  <--------------------------------> | ProductService.API|
   |    Controllers     |                                     |   Controllers    |
   +----------+---------+                                     +---------+--------+
              |                                                       |
          Application                                            Application
        (Services, DTOs,                                        (Services, DTOs,
         Validators)                                             Validators)
              |                                                       |
         Infrastructure                                         Infrastructure
         (EF + Repos)                                            (EF + Repos)
              |                                                       |
           OrdersDb                                                ProductsDb
           (SQL Server)                                            (SQL Server)
```

---

## 🧠 Decisiones técnicas tomadas

1. **Separación por bounded contexts** (Products/Orders) con **DB por servicio** para aislar dominios y facilitar escalado/despliegue independiente.
2. **EF Core + Repository Pattern** para encapsular queries y facilitar **tests** con doblete de datos (InMemory).
3. **Validación** con **FluentValidation** (paquete core + DI). Se evita el paquete de **auto-validación** de ASP.NET Core por estar deprecado; se aplican reglas manualmente en servicios/controllers para mayor control.
4. **Resiliencia**: `IHttpClientFactory` + **Polly** (WaitAndRetry) en OrderService → ProductService. Evita `HttpClient` singleton, maneja DNS refresh y pool.
5. **Error Handling** centralizado (middleware/handler) que produce `ProblemDetails` estándar. Regla simple para `ValidationException` (extensible).
6. **Health Checks** para readiness/liveness básicos.
7. **Swagger/OpenAPI** activado en Development para exploración rápida y exportación a Postman.
8. **Auto‑migraciones** en arranque (solo dev/demo) para simplificar la ejecución. En prod se recomienda pipeline controlado de migraciones.
9. **Tests**: capa de aplicación con Moq; repos con InMemory; controllers con mocks del cliente de productos. **Cobertura mínima del 80%** configurada con Coverlet.
10. **Docker**: `Dockerfile` multi‑stage por servicio + `docker-compose.yml` con `mssql` y red de contenedores. Variables de entorno para *connstrings* y base URL.

---

## 📚 Documentación de API / Postman Collection

### Swagger
- ProductService: **http://localhost:8080/swagger**
- OrderService: **http://localhost:8081/swagger**

Desde Swagger puedes **Exportar** una colección a Postman. Además, te dejamos una **colección Postman** lista:

- **EcommerceChallenge.postman_collection.json** → [Descargar](sandbox:/mnt/data/EcommerceChallenge.postman_collection.json)

### Endpoints principales (resumen)

**ProductService** (`/api/products`)
- `GET` listar (paginado: `page`, `pageSize`)
- `GET /<built-in function id>` obtener por id
- `POST` crear
- `PUT /<built-in function id>` actualizar
- `DELETE /<built-in function id>` eliminar
- `GET /health` health‑check

**OrderService** (`/api/orders`)
- `GET` listar (paginado)
- `GET /<built-in function id>` obtener por id
- `POST` crear (valida existencia y stock del producto vía ProductService)
- `PUT /<built-in function id>/status` actualizar estado
- `GET /health` health‑check

---

## 🔧 Troubleshooting
- **SQL Server tarda en iniciar**: `orderservice`/`productservice` dependen de `mssql` con `healthcheck`, espera algunos segundos.
- **Puertos ocupados**: cambia mapeos `8080`/`8081` en `docker-compose.yml`.
- **Migraciones**: si cambias el modelo, regenera con `dotnet ef migrations add <Name>` y `dotnet ef database update` (o deja que `Migrate()` aplique en dev).

---

© 2025 – Microservices Starter by Alejandro Gil
