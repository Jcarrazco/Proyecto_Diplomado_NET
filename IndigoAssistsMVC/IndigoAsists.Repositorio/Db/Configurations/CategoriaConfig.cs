using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAsists.Repositorio.Db.Configurations
{
    /// <summary>
    /// Configuración Fluent API para la entidad mCategoriasTicket
    /// </summary>
    public class CategoriaConfig : IEntityTypeConfiguration<mCategoriasTicket>
    {
        public void Configure(EntityTypeBuilder<mCategoriasTicket> builder)
        {
            builder.ToTable("mCategoriasTicket");

            // Configurar clave primaria
            builder.HasKey(c => c.IdCategoria);

            // Configurar propiedades
            builder.Property(c => c.IdCategoria)
                .HasColumnName("IdCategoria")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Categoria)
                .HasColumnName("Categoria")
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(c => c.IdDepto)
                .HasColumnName("IdDepto")
                .IsRequired();

            // Configurar relaciones
            builder.HasOne(c => c.Departamento)
                .WithMany()
                .HasForeignKey(c => c.IdDepto)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar índices
            builder.HasIndex(c => c.IdDepto)
                .HasDatabaseName("IX_Categorias_IdDepto");

            builder.HasIndex(c => c.Categoria)
                .HasDatabaseName("IX_Categorias_Categoria");
        }
    }

    /// <summary>
    /// Configuración Fluent API para la entidad mSubCategoriasTicket
    /// </summary>
    public class SubCategoriaConfig : IEntityTypeConfiguration<mSubCategoriasTicket>
    {
        public void Configure(EntityTypeBuilder<mSubCategoriasTicket> builder)
        {
            builder.ToTable("mSubCategoriasTicket");

            // Configurar clave primaria
            builder.HasKey(sc => sc.IdSubCategoria);

            // Configurar propiedades
            builder.Property(sc => sc.IdSubCategoria)
                .HasColumnName("IdSubCategoria")
                .ValueGeneratedOnAdd();

            builder.Property(sc => sc.SubCategoria)
                .HasColumnName("SubCategoria")
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(sc => sc.IdCategoria)
                .HasColumnName("IdCategoria")
                .IsRequired();

            // Configurar relaciones
            builder.HasOne(sc => sc.Categoria)
                .WithMany()
                .HasForeignKey(sc => sc.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar índices
            builder.HasIndex(sc => sc.IdCategoria)
                .HasDatabaseName("IX_SubCategorias_IdCategoria");

            builder.HasIndex(sc => sc.SubCategoria)
                .HasDatabaseName("IX_SubCategorias_SubCategoria");
        }
    }

    /// <summary>
    /// Configuración Fluent API para la entidad mDepartamentos
    /// </summary>
    public class DepartamentoConfig : IEntityTypeConfiguration<mDepartamentos>
    {
        public void Configure(EntityTypeBuilder<mDepartamentos> builder)
        {
            builder.ToTable("mDepartamentos");

            // Configurar clave primaria
            builder.HasKey(d => d.IdDepto);

            // Configurar propiedades
            builder.Property(d => d.IdDepto)
                .HasColumnName("IdDepto")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Departamento)
                .HasColumnName("Departamento")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Tickets)
                .HasColumnName("Tickets")
                .IsRequired()
                .HasDefaultValue(false);

            // Configurar índices
            builder.HasIndex(d => d.Departamento)
                .HasDatabaseName("IX_Departamentos_Departamento");
        }
    }
}
