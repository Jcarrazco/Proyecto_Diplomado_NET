# Manual de instalacion - IndigoAssist

Guia paso a paso para instalar y ejecutar la suite IndigoAssist (MVC + API) y, opcionalmente, la app MAUI.

## Requisitos

- .NET 8 SDK
- SQL Server (Indigo + IndigoBasic)
- Git
- (Opcional) Visual Studio 2022 o VS Code

## 1) Clonar el repositorio

```bash
git clone <URL_DEL_REPO>
```

## 2) Preparar bases de datos (SQL Server)

### 2.1 IndigoBasic (Identity)

Ejecuta en orden:

1. `IndigoAssistsMVC/ScriptBD/IndigoBasic_Nueva.sql`
2. `IndigoAssistsMVC/ScriptBD/IndigoBasic_Seeders.sql`

### 2.2 Indigo (legacy: tickets + activos)

Ejecuta en orden:

1. `IndigoAssistsMVC/SQL/Init.Project.sql`
2. (Alternativa) `IndigoAssistsMVC/SQL/Activos.Schema.sql`
3. (Alternativa) `IndigoAssistsMVC/SQL/Activos.Seeders.sql`
4. `IndigoAssistsMVC/SQL/Tickets.StoredProcedures.sql`

Notas:
- Si ya tienes Indigo con catalogos/tickets, ejecuta solo lo necesario.
- El SP `usp_Tickets_Insert_Human` es requerido para inserts humanizados.

## 3) Configurar cadenas de conexion

### MVC

Archivo: `IndigoAssistsMVC/IndigoAssistMVC/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=IndigoBasic;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "ApiSettings": {
    "BaseUrl": "http://localhost:5124"
  }
}
```

### API

Archivo: `IndigoAssistsMVC/IndigoAssits.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=IndigoBasic;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "LegacyConnectionStrings": {
    "GDL": "Server=TU_SERVIDOR;Database=Indigo;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "IndigoAssist",
    "Audience": "IndigoAssistClients",
    "Key": "REEMPLAZAR_POR_CLAVE_SECRETA_SEGURA",
    "AccessTokenMinutes": 60
  }
}
```

Notas:
- Si usas SQL Auth local, agrega `TrustServerCertificate=True` o `Encrypt=False`.
- Si usas Windows Auth, elimina `User Id`/`Password` y usa `Trusted_Connection=True`.
- Mantener coherentes las cadenas entre MVC y API.

## 4) Ejecutar migraciones de Identity (opcional)

```bash
dotnet ef database update --project "IndigoAssistsMVC/IndigoAsists.Repositorio" --startup-project "IndigoAssistsMVC/IndigoAssistMVC" --context "IndigoAsists.Repositorio.Db.IndigoDbContext"
```

Si la BD ya tiene tablas de Identity:

```bash
dotnet ef migrations add BaselineExistingDb --project "IndigoAssistsMVC/IndigoAsists.Repositorio" --startup-project "IndigoAssistsMVC/IndigoAssistMVC" --context "IndigoAsists.Repositorio.Db.IndigoDbContext" --ignore-changes
dotnet ef database update --project "IndigoAssistsMVC/IndigoAsists.Repositorio" --startup-project "IndigoAssistsMVC/IndigoAssistMVC" --context "IndigoAsists.Repositorio.Db.IndigoDbContext"
```

## 5) Ejecutar la API

```bash
cd IndigoAssistsMVC/IndigoAssits.API
dotnet run
```

API (por defecto):
- `http://localhost:5124`
- `https://localhost:7167`

Header requerido (legacy):

```
X-Sucursal: GDL
```

## 6) Ejecutar la web MVC

```bash
cd IndigoAssistsMVC/IndigoAssistMVC
dotnet run
```

MVC (por defecto):
- `http://localhost:5212`

## 7) Usuarios demo

- admin@indigo.com / Password123!
- supervisor@indigo.com / Password123!
- tecnico@indigo.com / Password123!
- usuario@indigo.com / Password123!

## 8) (Opcional) App MAUI

Sigue las instrucciones en `IndigoAssistsMAUI/README.md`.

## Problemas comunes

- **Swagger no abre**: revisa `IndigoAssits.API/Properties/launchSettings.json`.
- **Login falla**: confirma que MVC y API apuntan a la misma BD `IndigoBasic`.
- **TLS/SSL**: agrega `TrustServerCertificate=True` o `Encrypt=False`.
