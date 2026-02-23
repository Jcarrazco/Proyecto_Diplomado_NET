using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAsists.Repositorio.Db
{
    public class IndigoDbContext : IdentityDbContext<Usuario, Rol, string>
    {
        public IndigoDbContext(DbContextOptions<IndigoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.NombreCompleto)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.IdDepartamento)
                    .IsRequired(false);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("GETDATE()");

                entity.Ignore(e => e.TicketsSolucionados);
                entity.Ignore(e => e.TicketsEnProceso);
                entity.Ignore(e => e.TicketsPendientes);
                entity.Ignore(e => e.TiempoPromedioResolucion);
                entity.Ignore(e => e.Departamento);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.Ignore(e => e.Descripcion);
                entity.Ignore(e => e.Activo);
            });
        }
    }
}
