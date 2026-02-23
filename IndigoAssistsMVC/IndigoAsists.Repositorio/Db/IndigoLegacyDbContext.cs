using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAsists.Repositorio.Db
{
    public class IndigoLegacyDbContext : DbContext
    {
        public IndigoLegacyDbContext(DbContextOptions<IndigoLegacyDbContext> options) : base(options)
        {
        }

        #region DbSets - Entidades principales
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketVista> TicketVistas { get; set; }
        public DbSet<dTicketsTecnicos> TicketsTecnicos { get; set; }
        #endregion

        #region DbSets - Catalogos
        public DbSet<mDepartamentos> Departamentos { get; set; }
        public DbSet<mStatusTicket> StatusTickets { get; set; }
        public DbSet<mPrioridadTicket> PrioridadTickets { get; set; }
        public DbSet<mTipoTicket> TipoTickets { get; set; }
        public DbSet<mCategoriasTicket> CategoriasTickets { get; set; }
        public DbSet<mSubCategoriasTicket> SubCategoriasTickets { get; set; }
        public DbSet<mPersonas> Personas { get; set; }
        public DbSet<mEmpleados> Empleados { get; set; }
        public DbSet<mPerEmp> PersonasEmpresas { get; set; }
        public DbSet<dEmpleados> DetalleEmpleados { get; set; }
        public DbSet<mPuestos> Puestos { get; set; }
        #endregion

        #region DbSets - Activos
        public DbSet<Activo> Activos { get; set; }
        public DbSet<mTipoActivo> TiposActivo { get; set; }
        public DbSet<mStatus> Status { get; set; }
        public DbSet<mProveedor> Proveedores { get; set; }
        public DbSet<mComponente> Componentes { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureTicketEntities(modelBuilder);
            ConfigureActivoEntities(modelBuilder);
        }

        private static void ConfigureTicketEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketVista>(entity =>
            {
                entity.ToView("vTickets");
                entity.HasNoKey();
            });

            modelBuilder.Entity<mPerEmp>(entity =>
            {
                entity.ToTable("mPerEmp");
                entity.HasKey(e => e.IdPersona);
                entity.Property(e => e.IdPersona).ValueGeneratedOnAdd();

                entity.HasOne(e => e.PersonaInfo)
                    .WithMany()
                    .HasForeignKey(e => e.Persona)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<mPersonas>(entity =>
            {
                entity.ToTable("mPersonas");
                entity.HasKey(e => e.Persona);
                entity.Property(e => e.Persona).ValueGeneratedOnAdd();

                entity.HasOne(e => e.ReferenciaNavigation)
                    .WithMany()
                    .HasForeignKey(e => e.IdReferencia)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<mEmpleados>(entity =>
            {
                entity.ToTable("mEmpleados");
                entity.HasKey(e => e.IdPersona);

                entity.HasOne(e => e.PersonaEmpresa)
                    .WithMany()
                    .HasForeignKey(e => e.IdPersona)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<mDepartamentos>(entity =>
            {
                entity.ToTable("mDepartamentos");
                entity.HasKey(e => e.IdDepto);
                entity.Property(e => e.IdDepto)
                    .ValueGeneratedOnAdd()
                    .HasComment("Identificador unico del departamento");

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

            modelBuilder.Entity<mStatusTicket>(entity =>
            {
                entity.ToTable("mStatusTicket");
                entity.HasKey(e => e.Status);
                entity.Property(e => e.Status).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<mPrioridadTicket>(entity =>
            {
                entity.ToTable("mPrioridadTicket");
                entity.HasKey(e => e.IdPrioridad);
                entity.Property(e => e.IdPrioridad).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<mTipoTicket>(entity =>
            {
                entity.ToTable("mTipoTicket");
                entity.HasKey(e => e.IdTipoTicket);
                entity.Property(e => e.IdTipoTicket).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<mCategoriasTicket>(entity =>
            {
                entity.ToTable("mCategoriasTicket");
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.IdCategoria).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<mSubCategoriasTicket>(entity =>
            {
                entity.ToTable("mSubCategoriasTicket");
                entity.HasKey(e => e.IdSubCategoria);
                entity.Property(e => e.IdSubCategoria).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<dTicketsTecnicos>(entity =>
            {
                entity.ToTable("dTicketsTecnicos");
                entity.HasKey(e => new { e.IdTicket, e.IdPersona });
            });

            modelBuilder.Entity<dEmpleados>(entity =>
            {
                entity.ToTable("dEmpleados");
                entity.HasKey(e => new { e.IdPersona, e.IdPuesto });

                entity.HasOne(e => e.Empleado)
                    .WithMany()
                    .HasForeignKey(e => e.IdPersona)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Puesto)
                    .WithMany()
                    .HasForeignKey(e => e.IdPuesto)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<mPuestos>(entity =>
            {
                entity.ToTable("mPuestos");
                entity.HasKey(e => e.IdPuesto);
                entity.Property(e => e.IdPuesto).ValueGeneratedOnAdd();
            });

        }

        private static void ConfigureActivoEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activo>(entity =>
            {
                entity.ToTable("mActivos");
                entity.HasKey(e => e.IdActivo);
                entity.Property(e => e.IdActivo).ValueGeneratedOnAdd();

                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(40);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(120);
                entity.Property(e => e.FeAlta).IsRequired().HasColumnType("date");
                entity.Property(e => e.FeCompra).HasColumnType("date");
                entity.Property(e => e.FeBaja).HasColumnType("date");
                entity.Property(e => e.CostoCompra).HasColumnType("decimal(12,2)");
                entity.Property(e => e.CodificacionComponentes)
                    .IsRequired(false)
                    .HasDefaultValue(0);
                entity.Property(e => e.TieneSoftwareOP)
                    .IsRequired(false)
                    .HasDefaultValue(false);

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

            modelBuilder.Entity<mTipoActivo>(entity =>
            {
                entity.ToTable("mTiposActivo");
                entity.HasKey(e => e.IdTipoActivo);
                entity.Property(e => e.IdTipoActivo).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<mStatus>(entity =>
            {
                entity.ToTable("mStatusActivo");
                entity.HasKey(e => e.StatusId);
                entity.Property(e => e.StatusId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<mProveedor>(entity =>
            {
                entity.ToTable("mProveedoresActivos");
                entity.HasKey(e => e.IdProveedor);
                entity.Property(e => e.IdProveedor).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<mComponente>(entity =>
            {
                entity.ToTable("mComponentesActivos");
                entity.HasKey(e => e.IdComponente);
                entity.Property(e => e.IdComponente).ValueGeneratedOnAdd();
            });
        }
    }
}
