# Manual de uso - IndigoAssist MVC y MAUI

Guia rapida para operar la aplicacion web MVC y la app MAUI de activos.

## IndigoAssist MVC (Web)

### Acceso y sesion

1) Abre la web en `http://localhost:5212`.
2) Inicia sesion con un usuario demo (ver `INSTALL.md`) o tu usuario Identity.
3) Si no inicia sesion, confirma que MVC y API apuntan a la misma BD `IndigoBasic`.

### Navegacion principal

- **Tickets**: listado, detalle, creacion y gestion.
- **Activos**: listado, detalle, alta, edicion y baja.
- **Catalogos**: consulta de tipos, status, proveedores, categorias.

### Flujo de tickets

1) Crear ticket desde la pantalla de Tickets.
2) Asignar tecnico y prioridad.
3) Agregar anotaciones y seguimiento.
4) Cerrar o reabrir segun estado.

### Flujo de activos

1) Registrar activo con tipo, status, proveedor y departamento.
2) Editar informacion y estado.
3) Consultar historial y relacion con catalogos.

### Endpoints y API

- La web consume la API en `ApiSettings:BaseUrl`.
- Requiere JWT para operaciones protegidas.
- Header legacy requerido: `X-Sucursal: GDL`.

## ActivosApp MAUI

### Acceso y sesion

1) Ejecuta la app MAUI.
2) Inicia sesion con usuario valido del API MAUI.
3) El token JWT se guarda en `SessionService`.

### Funcionalidades principales

- **Listado** de activos con busqueda rapida.
- **Detalle** de activo.
- **Crear** activo.
- **Editar** activo.
- **Eliminar** activo.

### Flujo tipico

1) Login en `LoginPage`.
2) Listado en `ActivosVistaPage`.
3) Detalle en `ActivoDetallePage`.
4) Crear en `ActivoCreatePage`.
5) Editar en `ActivoEditPage`.
6) Borrar en `BorrarActivoPage`.

### API MAUI (externa)

- Base URL definida en `IndigoAssistsMAUI/ActivosApp/MauiProgram.cs`.
- Endpoints consumidos:
  - `POST auth/login`
  - `POST auth/register`
  - `GET activos`
  - `GET activos/{id}`
  - `POST activos`
  - `PUT activos/{id}`
  - `DELETE activos/{id}`

## Consejos de uso

- Si no ves datos, confirma que la BD legacy tenga catalogos cargados.
- Para tickets, verifica que existan los SPs y la vista `vTickets`.
- En entornos de prueba, usa URLs locales y configura CORS en la API.
