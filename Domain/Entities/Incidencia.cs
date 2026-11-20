namespace Domain.Entities;

public class Incidencia
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "Abierta";
    public DateTime FechaReporte { get; set; } = DateTime.UtcNow;

    // Relación con el catálogo
    public int DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }
    public string? ImagenRuta { get; set; }
    public string? ImagenContentType { get; set; }
    public string? VideoRuta { get; set; }
    public string? VideoContentType { get; set; }
}