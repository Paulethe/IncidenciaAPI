using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidenciasController(IIncidenciaService service) : ControllerBase
{
    private static readonly string[] EstadosValidos = ["Abierta", "EnProceso", "Cerrada"];

    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<IncidenciaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<IncidenciaDto>>> Get(
        [FromQuery] string? search,
        [FromQuery] string? estado,
        [FromQuery] int? departamentoId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1)
            throw new ValidacionException("page debe ser mayor o igual a 1.");
        if (pageSize is < 1 or > 100)
            throw new ValidacionException("pageSize debe estar entre 1 y 100.");
        if (!string.IsNullOrWhiteSpace(estado) && !EstadosValidos.Contains(estado))
            throw new ValidacionException("estado debe ser Abierta, EnProceso o Cerrada.");

        return Ok(await service.ListarAsync(search, estado, departamentoId, page, pageSize, ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(IncidenciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IncidenciaDto>> GetById(int id, CancellationToken ct) =>
        Ok(await service.ObtenerAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(typeof(IncidenciaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IncidenciaDto>> Post(CrearIncidenciaDto dto, CancellationToken ct)
    {
        var creada = await service.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put(int id, ActualizarIncidenciaDto dto, CancellationToken ct)
    {
        await service.ActualizarAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.EliminarAsync(id, ct);
        return NoContent();
    }
}