# Diagrama de Relaciones del Sistema de Tickets - COMPLETO

## Diagrama Entidad-Relación Actualizado

```
┌─────────────────────┐
│    mPersonas        │
│  - Persona (PK)     │
│  - Nombre           │
│  - Paterno          │
│  - Materno          │
│  - RFC              │
│  - Email            │
│  - TipoPersona      │
│  - IdReferencia     │
│  - Usuario          │
│  - FeModifica       │
└──────────┬──────────┘
           │
           │ Persona (auto-referencia)
           │
           ├────────────────────────┐
           │                        │
           │ Persona                │
           ▼                        ▼
┌─────────────────────┐    ┌─────────────────────┐
│    mEmpresas        │    │     mPerEmp         │
│  - IdEmpresa (PK)   │    │  - IdPersona (PK)   │
│  - Persona (FK)     │    │  - IdEmpresa (FK)   │
│  - Logo             │    │  - Persona (FK)     │
└──────────┬──────────┘    └──────────┬──────────┘
           │                          │
           │ IdEmpresa                │ Usuario
           │                          │
           └──────────┬───────────────┘
                      │
                      ▼
┌─────────────────────────────────────┐
│          mTickets                   │
│  - IdTicket (PK)                    │
│  - Usuario (FK → mPerEmp)           │
│  - IdSubCategoria (FK)              │
│  - Titulo                           │
│  - Descripcion                      │
│  - Status (FK)                      │
│  - IdTipoTicket (FK)                │
│  - Prioridad (FK)                   │
│  - FeAlta                           │
│  - FeAsignacion                     │
│  - FeCompromiso                     │
│  - FeCierre                         │
└──────────┬──────────────────────────┘
           │
           │ IdTicket
           │
           ├────────────────────────┐
           │                        │
           ▼                        ▼
┌─────────────────────┐  ┌─────────────────────┐
│     dTickets        │  │  mValoracionTicket  │
│  - IdDTicket (PK)   │  │  - IdTicket         │
│  - IdTicket (FK)    │  │  - Fecha            │
│  - Evento           │  │  - Valoracion       │
│  - Tiempo           │  │  - Comentario       │
│  - Fecha            │  └─────────────────────┘
│  - Usuario          │
└─────────────────────┘


┌─────────────────────┐
│     mSysVar         │
│  - Variable (PK)    │
│  - Valor            │
└─────────────────────┘
     │
     │ Variable
     │
     └──────► (Usado por GetSysVar)


┌─────────────────────┐
│  mDepartamentos     │
│  - IdDepto (PK)     │
│  - Departamento     │
│  - Tickets          │
└──────────┬──────────┘
           │
           │ IdDepto
           │
           ├────────────────────┐
           │                    │
           ▼                    ▼
┌─────────────────────┐  ┌─────────────────────┐
│     mPuestos        │  │ mCategoriasTicket   │
│  - IdPuesto (PK)    │  │  - IdCategoria (PK) │
│  - Puesto           │  │  - Categoria        │
│  - IdDepto (FK)     │  │  - IdDepto (FK)     │
└──────────┬──────────┘  └──────────┬──────────┘
           │                        │
           │ IdPuesto               │ IdCategoria
           ▼                        ▼
┌─────────────────────┐  ┌──────────────────────────┐
│    dEmpleados       │  │  mSubCategoriasTicket    │
│  - IdPersona        │  │  - IdSubCategoria (PK)   │
│  - IdPuesto (FK)    │  │  - SubCategoria          │
│  - Principal        │  │  - IdCategoria (FK)      │
└──────────┬──────────┘  └──────────┬───────────────┘
           │                        │
           │ IdPersona              │ IdSubCategoria
           ▼                        │
┌─────────────────────┐             │
│   mEmpleados        │             │
│  - IdPersona (PK)   │             │
│  - Login            │             │
│  - Activo           │             │
└──────────┬──────────┘             │
           │                        │
           │ IdPersona              │
           │                        │
           ├────────────────────────┼──────────────┐
           │                        │              │
           ▼                        ▼              ▼
┌─────────────────────┐  ┌─────────────────────┐  ┌─────────────────────┐
│ dTicketsTecnicos    │  │     mTickets        │  │     mTickets        │
│  - IdTicket (PK,FK) │  │  - IdTicket (PK)    │  │  - IdTicket (PK)    │
│  - IdPersona (PK,FK)│  │  - IdSubCategoria   │  │  - IdSubCategoria   │
└─────────────────────┘  │  - Status           │  │  - Status           │
                         │  - IdTipoTicket     │  │  - IdTipoTicket     │
                         │  - Prioridad        │  │  - Prioridad        │
                         └─────────────────────┘  └─────────────────────┘
                                   │                        │
                                   │                        │
                                   ▼                        ▼
                         ┌─────────────────────┐  ┌─────────────────────┐
                         │  mStatusTicket      │  │ mPrioridadTicket    │
                         │  - Status (PK)      │  │  - IdPrioridad (PK) │
                         │  - StatusDes        │  │  - Prioridad        │
                         └─────────────────────┘  └─────────────────────┘
                                   │                        
                                   │                        
                                   ▼                       
                         ┌─────────────────────┐  
                         │   mTipoTicket       │  
                         │  - IdTipoTicket(PK) │  
                         │  - TipoTicket       │  
                         └─────────────────────┘  
```

## Tablas Históricas (Espejo de las activas)

```
┌─────────────────────────────────────┐
│          hTickets                   │
│  - IdTicket (PK)                    │
│  - Usuario (FK → mPerEmp)           │
│  - IdSubCategoria (FK)              │
│  - Titulo                           │
│  - Descripcion                      │
│  - IdTipoTicket (FK)                │
│  - Prioridad (FK)                   │
│  - FeAlta                           │
│  - FeAsignacion                     │
│  - FeCompromiso                     │
│  - FeCierre                         │
└──────────┬──────────────────────────┘
           │
           │ IdTicket
           │
           ├────────────────────┐
           │                    │
           ▼                    ▼
┌─────────────────────┐  ┌─────────────────────┐
│     hdTickets       │  │ hdTicketsTecnicos   │
│  - IdDTicket (PK)   │  │  - IdTicket (FK)    │
│  - IdTicket (FK)    │  │  - IdPersona (FK)   │
│  - Evento           │  └─────────────────────┘
│  - Tiempo           │
│  - Fecha            │
│  - Usuario          │
└─────────────────────┘
```

## Nuevas Tablas Incluidas

### mPersonas
- **Propósito**: Tabla base para personas físicas y morales
- **Campos clave**: Persona (PK), Nombre, RFC, Email, TipoPersona
- **Auto-referencia**: IdReferencia → Persona (para personas relacionadas)

### mEmpresas  
- **Propósito**: Empresas del sistema
- **Relación**: Persona → mPersonas.Persona
- **Campos**: IdEmpresa (PK), Persona (FK), Logo

### mSysVar
- **Propósito**: Variables de configuración del sistema
- **Campos**: Variable (PK), Valor
- **Uso**: Configuración de parámetros del sistema

## Funciones Incluidas

### GetSysVar
- **Propósito**: Obtiene valores de variables del sistema
- **Parámetros**: @variable NVARCHAR(20)
- **Retorna**: Valor de la variable NVARCHAR(50)
- **Uso**: Utilizada por TicketValoracionEnviar para obtener configuración

## Relaciones Actualizadas

### Jerarquía Completa
```
mPersonas (base)
    ├── mEmpresas (Persona → mPersonas.Persona)
    └── mPerEmp (Persona → mPersonas.Persona)
            └── mTickets (Usuario → mPerEmp.IdPersona)
```

### Sistema de Tickets Completo
```
mTickets
    ├── mStatusTicket (Status)
    ├── mPrioridadTicket (Prioridad)  
    ├── mTipoTicket (IdTipoTicket)
    ├── mSubCategoriasTicket (IdSubCategoria)
    │   └── mCategoriasTicket (IdCategoria)
    │       └── mDepartamentos (IdDepto)
    ├── dTickets (IdTicket) - Log de eventos
    ├── dTicketsTecnicos (IdTicket) - Técnicos asignados
    │   └── mEmpleados (IdPersona)
    │       └── dEmpleados (IdPersona)
    │           └── mPuestos (IdPuesto)
    │               └── mDepartamentos (IdDepto)
    └── mValoracionTicket (IdTicket) - Valoración
```

## Beneficios del Script Completo

1. **Sin Dependencias Externas**: El script es completamente autocontenido
2. **Sistema Completo**: Incluye todas las tablas necesarias para el funcionamiento
3. **Configuración Incluida**: mSysVar permite configurar parámetros del sistema
4. **Datos de Prueba**: Puede agregarse información de prueba en todas las tablas
5. **Funcionalidad Completa**: Todas las funciones y procedimientos funcionan correctamente

## Variables del Sistema Recomendadas

Para el funcionamiento del sistema de tickets, se recomienda configurar estas variables en mSysVar:

```sql
INSERT INTO mSysVar (Variable, Valor) VALUES
('TicketNoEncuestas', '2'),           -- Número máximo de encuestas por día
('TicketTiempoMaximo', '480'),        -- Tiempo máximo en minutos para resolver
('TicketNotificacionEmail', '1'),     -- Habilitar notificaciones por email
('TicketAutoAsignacion', '0');        -- Auto-asignación de tickets
```

---

**Nota**: Este diagrama representa el esquema completo del sistema de tickets sin dependencias externas. Para el esquema físico completo, consultar el archivo `Tickets_Isolated_Script.sql`.

