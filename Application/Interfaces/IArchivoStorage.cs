namespace Application.Interfaces;

public interface IArchivoStorage
{
    Task<string> GuardarAsync(string carpeta, string extension, Stream contenido, CancellationToken ct = default);
    Stream? Abrir(string rutaRelativa);
    void Eliminar(string? rutaRelativa);
}