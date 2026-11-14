using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CrearIncidenciaDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, MinimumLength = 5, ErrorMessage = "El título debe tener entre 5 y 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un departamento válido.")]
    public int DepartamentoId { get; set; }
}

public class ActualizarIncidenciaDto : CrearIncidenciaDto
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [RegularExpression("^(Abierta|EnProceso|Cerrada)$",
        ErrorMessage = "El estado debe ser Abierta, EnProceso o Cerrada.")]
    public string Estado { get; set; } = string.Empty;
}

public record IncidenciaDto(
    int Id, string Titulo, string Descripcion, string Estado,
    DateTime FechaReporte, int DepartamentoId, string? DepartamentoNombre);

public record ResultadoPaginado<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);