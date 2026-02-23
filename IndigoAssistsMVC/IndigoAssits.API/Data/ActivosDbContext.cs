using IndigoAssits.Repositorio.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace IndigoAssits.API.Data
{
    public class ActivosDbContext : DbContext
    {
        public ActivosDbContext(DbContextOptions<ActivosDbContext> options) : base(options)
        {
        }

        public DbSet<Activo> Activos { get; set; }
        public DbSet<mTipoActivo> TiposActivo { get; set; }
        public DbSet<mStatus> Status { get; set; }
        public DbSet<mProveedor> Proveedores { get; set; }
        public DbSet<mComponente> Componentes { get; set; }
        public DbSet<mDepartamentos> Departamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<mDepartamentos>(entity =>
            {
                entity.ToTable("mDepartamentos");
                entity.HasKey(e => e.IdDepto);
                entity.Property(e => e.IdDepto).ValueGeneratedOnAdd();
            });
        }
    }
}
