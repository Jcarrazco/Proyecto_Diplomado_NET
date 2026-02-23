# IndigoAssist API

API REST para tickets, activos y catalogos. Tickets y activos se gestionan en la BD legacy Indigo. Identity usa IndigoBasic.

## Requisitos
- .NET 8 SDK
- SQL Server con BD Indigo (legacy) y BD IndigoBasic
- La preparacion SQL se ejecuta desde MVC (ver README principal)

## Configuracion
Archivo: `IndigoAssits.API/appsettings.json`
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
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:5212" ]
  }
}
```

Header requerido para consultas legacy:
```
X-Sucursal: GDL
```

## Ejecucion
```bash
cd IndigoAssits.API
dotnet run
```

Swagger:
- `http://localhost:5124/swagger`
- `https://localhost:7167/swagger` (solo perfil https)

## Endpoints

### Auth
- `POST /api/auth/login` login y emite JWT

### Tickets (requiere JWT)
- `GET /api/tickets` lista paginada por filtros
- `GET /api/tickets/abiertos` tickets abiertos
- `GET /api/tickets/{id}` detalle de ticket
- `POST /api/tickets` crear ticket
- `PUT /api/tickets/{id}` actualizar ticket
- `PATCH /api/tickets/{id}/asignar` asignar tecnicos
- `PATCH /api/tickets/{id}/desasignar` quitar asignacion
- `PATCH /api/tickets/{id}/cerrar` cerrar ticket
- `PATCH /api/tickets/{id}/reabrir` reabrir ticket
- `PATCH /api/tickets/{id}/estado/{estado}` cambiar estado (0-255)
- `POST /api/tickets/{id}/anotaciones` agregar anotacion
- `GET /api/tickets/estadisticas` resumen por departamento
- `GET /api/tickets/dashboard` resumen por usuario/depto
- `GET /api/tickets/buscar?q=texto` busqueda libre
- `GET /api/tickets/recientes?cantidad=10` ultimos tickets

### Activos (requiere JWT)
- `GET /api/activos` lista paginada por filtros
- `GET /api/activos/todos` lista completa
- `GET /api/activos/{id}` detalle
- `POST /api/activos` crear activo
- `PUT /api/activos/{id}` actualizar activo
- `DELETE /api/activos/{id}` eliminar activo

### Catalogo (requiere JWT)
- `GET /api/catalogo/tipos-activo` tipos de activo
- `GET /api/catalogo/status-activo` status de activo
- `GET /api/catalogo/proveedores` proveedores
- `GET /api/catalogo/componentes` componentes
- `GET /api/catalogo/departamentos` departamentos

### Catalogo Tickets (requiere JWT)
- `GET /api/catalogo/tickets/status` status de ticket
- `GET /api/catalogo/tickets/prioridades` prioridades
- `GET /api/catalogo/tickets/tipos` tipos de ticket
- `GET /api/catalogo/tickets/categorias` categorias
- `GET /api/catalogo/tickets/subcategorias` subcategorias
- `GET /api/catalogo/tickets/subcategorias/{idCategoria}` subcategorias por categoria

### Categorias
- `GET /api/categoria` categorias
- `GET /api/categoria/departamento/{id}` categorias por departamento

### Usuarios (requiere JWT)
- `GET /api/usuarios/contexto` contexto del usuario (depto/persona)
- `GET /api/usuarios/tecnicos?departamentoId=1` tecnicos activos (opcional por departamento)
- `POST /api/usuarios/legacy/ensure` crea/vincula usuario legacy por email y login
