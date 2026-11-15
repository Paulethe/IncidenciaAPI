using Domain.Entities;

namespace Application.Interfaces;

public interface IIncidenciaRepository
{
    Task<(IReadOnlyList<Incidencia> Items, int Total)> ListarAsync(
        string? search, string? estado, int? departamentoId,
        int page, int pageSize, CancellationToken cancellationToken = default);

    Task<Incidencia?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExisteDepartamentoAsync(int departamentoId, CancellationToken cancellationToken = default);
    Task<bool> ExisteTituloAsync(string titulo, int departamentoId, int? excluirId,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(Incidencia incidencia, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Incidencia incidencia, CancellationToken cancellationToken = default);
    Task EliminarAsync(Incidencia incidencia, CancellationToken cancellationToken = default);
}