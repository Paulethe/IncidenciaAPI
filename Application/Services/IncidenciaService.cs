using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public interface IIncidenciaService
{
    Task<ResultadoPaginado<IncidenciaDto>> ListarAsync(string? search, string? estado,
        int? departamentoId, int page, int pageSize, CancellationToken ct = default);
    Task<IncidenciaDto> ObtenerAsync(int id, CancellationToken ct = default);
    Task<IncidenciaDto> CrearAsync(CrearIncidenciaDto dto, CancellationToken ct = default);
    Task ActualizarAsync(int id, ActualizarIncidenciaDto dto, CancellationToken ct = default);
    Task EliminarAsync(int id, CancellationToken ct = default);
}

public class IncidenciaService(IIncidenciaRepository repo) : IIncidenciaService
{
    public async Task<ResultadoPaginado<IncidenciaDto>> ListarAsync(string? search, string? estado,
        int? departamentoId, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await repo.ListarAsync(search, estado, departamentoId, page, pageSize, ct);
        return new ResultadoPaginado<IncidenciaDto>(items.Select(ADto), total, page, pageSize);
    }

    public async Task<IncidenciaDto> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var incidencia = await repo.ObtenerPorIdAsync(id, ct)
            ?? throw new NotFoundException($"No existe una incidencia con id {id}.");
        return ADto(incidencia);
    }

    public async Task<IncidenciaDto> CrearAsync(CrearIncidenciaDto dto, CancellationToken ct = default)
    {
        await ValidarReglasAsync(dto, null, ct);

        var incidencia = new Incidencia
        {
            Titulo = dto.Titulo.Trim(),
            Descripcion = dto.Descripcion.Trim(),
            DepartamentoId = dto.DepartamentoId,
            Estado = "Abierta",
            FechaReporte = DateTime.UtcNow
        };

        await repo.AgregarAsync(incidencia, ct);
        return await ObtenerAsync(incidencia.Id, ct); // recarga con el nombre del departamento
    }

    public async Task ActualizarAsync(int id, ActualizarIncidenciaDto dto, CancellationToken ct = default)
    {
        var incidencia = await repo.ObtenerPorIdAsync(id, ct)
            ?? throw new NotFoundException($"No existe una incidencia con id {id}.");

        await ValidarReglasAsync(dto, id, ct);

        incidencia.Titulo = dto.Titulo.Trim();
        incidencia.Descripcion = dto.Descripcion.Trim();
        incidencia.DepartamentoId = dto.DepartamentoId;
        incidencia.Estado = dto.Estado;

        await repo.ActualizarAsync(incidencia, ct);
    }

    public async Task EliminarAsync(int id, CancellationToken ct = default)
    {
        var incidencia = await repo.ObtenerPorIdAsync(id, ct)
            ?? throw new NotFoundException($"No existe una incidencia con id {id}.");
        await repo.EliminarAsync(incidencia, ct);
    }

    private async Task ValidarReglasAsync(CrearIncidenciaDto dto, int? excluirId, CancellationToken ct)
    {
        if (!await repo.ExisteDepartamentoAsync(dto.DepartamentoId, ct))
            throw new ValidacionException($"El departamento {dto.DepartamentoId} no existe.");

        if (await repo.ExisteTituloAsync(dto.Titulo.Trim(), dto.DepartamentoId, excluirId, ct))
            throw new ConflictException("Ya existe una incidencia con ese título en el departamento.");
    }

    private static IncidenciaDto ADto(Incidencia i) => new(
        i.Id, i.Titulo, i.Descripcion, i.Estado, i.FechaReporte,
        i.DepartamentoId, i.Departamento?.Nombre);
}