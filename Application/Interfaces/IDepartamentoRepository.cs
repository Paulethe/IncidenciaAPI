using Domain.Entities;

namespace Application.Interfaces;

public interface IDepartamentoRepository
{
    Task<IReadOnlyList<Departamento>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
}