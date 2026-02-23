using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAsists.Repositorio.Db.Configurations
{
    /// <summary>
    /// Configuración Fluent API para la entidad Ticket
    /// </summary>
    public class TicketConfig : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("mTickets");

            // Configurar clave primaria
            builder.HasKey(t => t.IdTicket);

            // Configurar propiedades
            builder.Property(t => t.IdTicket)
                .HasColumnName("IdTicket")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Usuario)
                .HasColumnName("Usuario")
                .IsRequired();

            builder.Property(t => t.IdSubCategoria)
                .HasColumnName("IdSubCategoria")
                .IsRequired();

            builder.Property(t => t.Titulo)
                .HasColumnName("Titulo")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Descripcion)
                .HasColumnName("Descripcion")
                .IsRequired();

            builder.Property(t => t.Status)
                .HasColumnName("Status")
                .IsRequired();

            builder.Property(t => t.IdTipoTicket)
                .HasColumnName("IdTipoTicket");

            builder.Property(t => t.Prioridad)
                .HasColumnName("Prioridad");

            builder.Property(t => t.FeAlta)
                .HasColumnName("FeAlta")
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.FeAsignacion)
                .HasColumnName("FeAsignacion");

            builder.Property(t => t.FeCompromiso)
                .HasColumnName("FeCompromiso");

            builder.Property(t => t.FeCierre)
                .HasColumnName("FeCierre");

            // Configurar relaciones
            builder.HasOne(t => t.UsuarioSolicitante)
                .WithMany()
                .HasForeignKey(t => t.Usuario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.SubCategoria)
                .WithMany()
                .HasForeignKey(t => t.IdSubCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Estado)
                .WithMany()
                .HasForeignKey(t => t.Status)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TipoTicket)
                .WithMany()
                .HasForeignKey(t => t.IdTipoTicket)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.PrioridadTicket)
                .WithMany()
                .HasForeignKey(t => t.Prioridad)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar índices
            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Tickets_Status");

            builder.HasIndex(t => t.Usuario)
                .HasDatabaseName("IX_Tickets_Usuario");

            builder.HasIndex(t => t.FeAlta)
                .HasDatabaseName("IX_Tickets_FeAlta");

            builder.HasIndex(t => new { t.Status, t.Prioridad })
                .HasDatabaseName("IX_Tickets_Status_Prioridad");
        }
    }
}
