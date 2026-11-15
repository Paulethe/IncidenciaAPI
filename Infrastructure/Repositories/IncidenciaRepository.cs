using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class IncidenciaRepository(AppDbContext context) : IIncidenciaRepository
{
    public async Task<(IReadOnlyList<Incidencia> Items, int Total)> ListarAsync(
        string? search, string? estado, int? departamentoId,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = context.Incidencias
            .AsNoTracking()
            .Include(i => i.Departamento)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var texto = search.Trim();
            query = query.Where(i => i.Titulo.Contains(texto) || i.Descripcion.Contains(texto));
        }

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(i => i.Estado == estado);

        if (departamentoId.HasValue)
            query = query.Where(i => i.DepartamentoId == departamentoId.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.FechaReporte)
            .ThenByDescending(i => i.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<Incidencia?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Incidencias
            .Include(i => i.Departamento)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public Task<bool> ExisteDepartamentoAsync(int departamentoId, CancellationToken cancellationToken = default) =>
        context.Departamentos.AnyAsync(d => d.Id == departamentoId, cancellationToken);

    public Task<bool> ExisteTituloAsync(string titulo, int departamentoId, int? excluirId,
        CancellationToken cancellationToken = default) =>
        context.Incidencias.AnyAsync(i =>
            i.DepartamentoId == departamentoId &&
            i.Titulo == titulo &&
            (excluirId == null || i.Id != excluirId), cancellationToken);

    public async Task AgregarAsync(Incidencia incidencia, CancellationToken cancellationToken = default)
    {
        context.Incidencias.Add(incidencia);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ActualizarAsync(Incidencia incidencia, CancellationToken cancellationToken = default)
    {
        // La entidad ya viene rastreada desde ObtenerPorIdAsync
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task EliminarAsync(Incidencia incidencia, CancellationToken cancellationToken = default)
    {
        context.Incidencias.Remove(incidencia);
        await context.SaveChangesAsync(cancellationToken);
    }
}