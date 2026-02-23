using Microsoft.EntityFrameworkCore;
using IndigoAssistMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IndigoAssistMVC.Data
{
    public class IndigoDBContext : IdentityDbContext<Usuario> 
    {
        public IndigoDBContext(DbContextOptions<IndigoDBContext> options) : base(options)
        {
        }

        public DbSet<Activo> Activos { get; set; }
        public DbSet<TipoActivo> TiposActivo { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Componente> Componentes { get; set; }
        public DbSet<Software> Software { get; set; }
        
        // Tablas del sistema de Tickets
        public DbSet<TicketVista> TicketsVista { get; set; }
        public DbSet<mPerEmp> mPerEmp { get; set; }
        public DbSet<mPersonas> mPersonas { get; set; }
        public DbSet<mEmpleados> mEmpleados { get; set; }
        public DbSet<mDepartamentos> mDepartamentos { get; set; }
        public DbSet<mStatusTicket> mStatusTicket { get; set; }
        public DbSet<mPrioridadTicket> mPrioridadTicket { get; set; }
        public DbSet<mTipoTicket> mTipoTicket { get; set; }
        public DbSet<mCategoriasTicket> mCategoriasTicket { get; set; }
        public DbSet<mSubCategoriasTicket> mSubCategoriasTicket { get; set; }
        public DbSet<dTicketsTecnicos> dTicketsTecnicos { get; set; }
        public DbSet<mEmpresas> mEmpresas { get; set; }
        public DbSet<dEmpleados> dEmpleados { get; set; }
        public DbSet<mPuestos> mPuestos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Identity
            modelBuilder.Entity<Usuario>(entity =>
            {
                // Configurar propiedades personalizadas
                entity.Property(e => e.NombreCompleto)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasComment("Nombre completo del usuario");

                entity.Property(e => e.IdDepartamento)
                    .HasComment("ID del departamento");

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true)
                    .HasComment("Indica si el usuario está activo");

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("GETDATE()")
                    .HasComment("Fecha de registro del usuario");

                entity.Property(e => e.UltimoAcceso)
                    .HasComment("Último acceso del usuario");

                // NOTA: Temporalmente comentada la relación con Departamento
                // para crear migración solo de Identity. Descomentar después.
                // entity.HasOne(e => e.Departamento)
                //     .WithMany()
                //     .HasForeignKey(e => e.IdDepartamento)
                //     .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad Activo usando Fluent API
            modelBuilder.Entity<Activo>(entity =>
            {
                entity.ToTable("mActivos");

                entity.HasKey(e => e.IdActivo);
                entity.Property(e => e.IdActivo)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador único del activo");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(true)
                    .HasComment("Código único del activo");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(true)
                    .HasComment("Nombre descriptivo del activo");

                entity.Property(e => e.FeAlta)
                    .IsRequired()
                    .HasColumnType("date")
                    .HasComment("Fecha de alta del activo");

                entity.Property(e => e.Marca)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .HasComment("Marca del activo");

                entity.Property(e => e.Modelo)
                    .HasMaxLength(80)
                    .IsUnicode(true)
                    .HasComment("Modelo del activo");

                entity.Property(e => e.Serie)
                    .HasMaxLength(80)
                    .IsUnicode(true)
                    .HasComment("Número de serie del activo");

                entity.Property(e => e.PersonaAsign)
                    .HasMaxLength(120)
                    .IsUnicode(true)
                    .HasComment("Persona asignada al activo");

                entity.Property(e => e.Ubicacion)
                    .HasMaxLength(120)
                    .IsUnicode(true)
                    .HasComment("Ubicación física del activo");

                entity.Property(e => e.FeCompra)
                    .HasColumnType("date")
                    .HasComment("Fecha de compra del activo");

                entity.Property(e => e.FeBaja)
                    .HasColumnType("date")
                    .HasComment("Fecha de baja del activo");

                entity.Property(e => e.CostoCompra)
                    .HasColumnType("decimal(12,2)")
                    .HasComment("Costo de compra del activo");

                entity.Property(e => e.Notas)
                    .HasMaxLength(400)
                    .IsUnicode(true)
                    .HasComment("Notas adicionales sobre el activo");

                entity.Property(e => e.CodificacionComponentes)
                    .HasComment("Codificación de componentes");

                entity.Property(e => e.TieneSoftwareOP)
                    .HasComment("Indica si tiene software OP");

                // Configuración de las claves foráneas
                entity.Property(e => e.IdTipoActivo)
                    .HasComment("Tipo de activo");

                entity.Property(e => e.IdDepartamento)
                    .HasComment("Departamento");

                entity.Property(e => e.IdStatus)
                    .HasComment("Status del activo");

                entity.Property(e => e.IdProveedor)
                    .HasComment("Proveedor");

                // Configuración de las relaciones
                entity.HasOne(e => e.TipoActivo)
                    .WithMany(t => t.Activos)
                    .HasForeignKey(e => e.IdTipoActivo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Departamento)
                    .WithMany()
                    .HasForeignKey(e => e.IdDepartamento)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Status)
                    .WithMany(s => s.Activos)
                    .HasForeignKey(e => e.IdStatus)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Proveedor)
                    .WithMany(p => p.Activos)
                    .HasForeignKey(e => e.IdProveedor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de las entidades de catálogo
            ConfigureCatalogEntities(modelBuilder);

            // Configuración de las entidades del sistema de Tickets
            ConfigureTicketEntities(modelBuilder);
        }

        private void ConfigureCatalogEntities(ModelBuilder modelBuilder)
        {
            // Configuración de TipoActivo
            modelBuilder.Entity<TipoActivo>(entity =>
            {
                entity.ToTable("mTiposActivo");
                entity.HasKey(e => e.IdTipoActivo);
                entity.Property(e => e.IdTipoActivo)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador único del tipo de activo");
                
                entity.Property(e => e.TipoActivoNombre)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .HasComment("Nombre del tipo de activo");
            });


            // Configuración de Status
            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("mStatus");
                entity.HasKey(e => e.StatusId);
                entity.Property(e => e.StatusId)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador único del status");
                
                entity.Property(e => e.StatusNombre)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(true)
                    .HasComment("Nombre del status");
            });

            // Configuración de Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("mProveedores");
                entity.HasKey(e => e.IdProveedor);
                entity.Property(e => e.IdProveedor)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador único del proveedor");
                
                entity.Property(e => e.ProveedorNombre)
                    .IsRequired()
                    .HasMaxLength(120)
                    .IsUnicode(true)
                    .HasComment("Nombre del proveedor");
            });

            // Configuración de Componente
            modelBuilder.Entity<Componente>(entity =>
            {
                entity.ToTable("mComponentes");
                entity.HasKey(e => e.IdComponente);
                entity.Property(e => e.IdComponente)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador único del componente");
                
                entity.Property(e => e.ComponenteNombre)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(true)
                    .HasComment("Nombre del componente");
                
                entity.Property(e => e.ValorBit)
                    .HasComment("Valor bit para codificación");
            });

            // Configuración de Software
            modelBuilder.Entity<Software>(entity =>
            {
                entity.ToTable("mSoftware");
                entity.HasKey(e => e.IdSoftware);
                entity.Property(e => e.IdSoftware)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador único del software");
                
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(true)
                    .HasComment("Nombre del software");
            });
        }

        private void ConfigureTicketEntities(ModelBuilder modelBuilder)
        {
            // Configuración de TicketVista (vista)
            modelBuilder.Entity<TicketVista>(entity =>
            {
                entity.ToView("vTickets");
                entity.HasNoKey();
            });

            // Configuración de mPerEmp
            modelBuilder.Entity<mPerEmp>(entity =>
            {
                entity.ToTable("mPerEmp");
                entity.HasKey(e => e.IdPersona);
                entity.Property(e => e.IdPersona).ValueGeneratedOnAdd();
                
                // Configurar relaciones para evitar ciclos de cascada
                entity.HasOne(e => e.Empresa)
                    .WithMany()
                    .HasForeignKey(e => e.IdEmpresa)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.PersonaInfo)
                    .WithMany()
                    .HasForeignKey(e => e.Persona)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de mPersonas
            modelBuilder.Entity<mPersonas>(entity =>
            {
                entity.ToTable("mPersonas");
                entity.HasKey(e => e.Persona);
                entity.Property(e => e.Persona).ValueGeneratedOnAdd();
                
                // Configurar auto-referencia para evitar ciclos de cascada
                entity.HasOne(e => e.ReferenciaNavigation)
                    .WithMany()
                    .HasForeignKey(e => e.IdReferencia)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de mEmpleados
            modelBuilder.Entity<mEmpleados>(entity =>
            {
                entity.ToTable("mEmpleados");
                entity.HasKey(e => e.IdPersona);
                
                // Configurar relación con mPersonas para evitar ciclos de cascada
                entity.HasOne(e => e.PersonaEmpresa)
                    .WithMany()
                    .HasForeignKey(e => e.IdPersona)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de mDepartamentos
            modelBuilder.Entity<mDepartamentos>(entity =>
            {
                entity.ToTable("mDepartamentos");
                entity.HasKey(e => e.IdDepto);
                entity.Property(e => e.IdDepto)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador único del departamento");
                
                entity.Property(e => e.Departamento)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .HasComment("Nombre del departamento");
                
                entity.Property(e => e.Tickets)
                    .IsRequired()
                    .HasDefaultValue(false)
                    .HasComment("Indica si el departamento acepta tickets");
            });

            // Configuración de mStatusTicket
            modelBuilder.Entity<mStatusTicket>(entity =>
            {
                entity.ToTable("mStatusTicket");
                entity.HasKey(e => e.Status);
                entity.Property(e => e.Status).ValueGeneratedOnAdd();
            });

            // Configuración de mPrioridadTicket
            modelBuilder.Entity<mPrioridadTicket>(entity =>
            {
                entity.ToTable("mPrioridadTicket");
                entity.HasKey(e => e.IdPrioridad);
                entity.Property(e => e.IdPrioridad).ValueGeneratedOnAdd();
            });

            // Configuración de mTipoTicket
            modelBuilder.Entity<mTipoTicket>(entity =>
            {
                entity.ToTable("mTipoTicket");
                entity.HasKey(e => e.IdTipoTicket);
                entity.Property(e => e.IdTipoTicket).ValueGeneratedOnAdd();
            });

            // Configuración de mCategoriasTicket
            modelBuilder.Entity<mCategoriasTicket>(entity =>
            {
                entity.ToTable("mCategoriasTicket");
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.IdCategoria).ValueGeneratedOnAdd();
            });

            // Configuración de mSubCategoriasTicket
            modelBuilder.Entity<mSubCategoriasTicket>(entity =>
            {
                entity.ToTable("mSubCategoriasTicket");
                entity.HasKey(e => e.IdSubCategoria);
                entity.Property(e => e.IdSubCategoria).ValueGeneratedOnAdd();
            });

            // Configuración de dTicketsTecnicos
            modelBuilder.Entity<dTicketsTecnicos>(entity =>
            {
                entity.ToTable("dTicketsTecnicos");
                entity.HasKey(e => new { e.IdTicket, e.IdPersona });
            });

            // Configuración de mEmpresas
            modelBuilder.Entity<mEmpresas>(entity =>
            {
                entity.ToTable("mEmpresas");
                entity.HasKey(e => e.IdEmpresa);
                entity.Property(e => e.IdEmpresa).ValueGeneratedOnAdd();
                
                // Configurar relación con mPersonas para evitar ciclos de cascada
                entity.HasOne(e => e.PersonaInfo)
                    .WithMany()
                    .HasForeignKey(e => e.Persona)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de dEmpleados
            modelBuilder.Entity<dEmpleados>(entity =>
            {
                entity.ToTable("dEmpleados");
                entity.HasKey(e => new { e.IdPersona, e.IdPuesto });
                
                // Configurar relaciones para evitar ciclos de cascada
                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdPersona)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.Puesto)
                    .WithMany()
                    .HasForeignKey(e => e.IdPuesto)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configuración de mPuestos
            modelBuilder.Entity<mPuestos>(entity =>
            {
                entity.ToTable("mPuestos");
                entity.HasKey(e => e.IdPuesto);
                entity.Property(e => e.IdPuesto).ValueGeneratedOnAdd();
            });
        }
    }
}
