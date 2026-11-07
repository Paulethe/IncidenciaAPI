using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly AppDbContext _context;

    public DepartamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Departamento>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Departamentos
            .AsNoTracking()
            .OrderBy(d => d.Nombre)
            .ToListAsync(cancellationToken);
    }
}