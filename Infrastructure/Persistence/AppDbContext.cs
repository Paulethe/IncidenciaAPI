using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Incidencia> Incidencias => Set<Incidencia>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Departamento>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Nombre).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Incidencia>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Titulo).IsRequired().HasMaxLength(150);
            e.Property(i => i.Descripcion).IsRequired().HasMaxLength(1000);
            e.Property(i => i.Estado).IsRequired().HasMaxLength(30);

            e.HasOne(i => i.Departamento)
             .WithMany(d => d.Incidencias)
             .HasForeignKey(i => i.DepartamentoId);
        });

        modelBuilder.Entity<Departamento>().HasData(
    new Departamento { Id = 1, Nombre = "Atlántida" },
    new Departamento { Id = 2, Nombre = "Choluteca" },
    new Departamento { Id = 3, Nombre = "Colón" },
    new Departamento { Id = 4, Nombre = "Comayagua" },
    new Departamento { Id = 5, Nombre = "Copán" },
    new Departamento { Id = 6, Nombre = "Cortés" },
    new Departamento { Id = 7, Nombre = "El Paraíso" },
    new Departamento { Id = 8, Nombre = "Francisco Morazán" },
    new Departamento { Id = 9, Nombre = "Gracias a Dios" },
    new Departamento { Id = 10, Nombre = "Intibucá" },
    new Departamento { Id = 11, Nombre = "Islas de la Bahía" },
    new Departamento { Id = 12, Nombre = "La Paz" },
    new Departamento { Id = 13, Nombre = "Lempira" },
    new Departamento { Id = 14, Nombre = "Ocotepeque" },
    new Departamento { Id = 15, Nombre = "Olancho" },
    new Departamento { Id = 16, Nombre = "Santa Bárbara" },
    new Departamento { Id = 17, Nombre = "Valle" },
    new Departamento { Id = 18, Nombre = "Yoro" }
);
    }
}