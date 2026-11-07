namespace Domain.Entities;

public class Departamento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Un departamento tiene muchas incidencias
    public ICollection<Incidencia> Incidencias { get; set; } = new List<Incidencia>();
}