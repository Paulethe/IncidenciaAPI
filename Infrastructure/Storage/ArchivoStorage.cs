using Application.Interfaces;

namespace Infrastructure.Storage;

public class ArchivoStorage : IArchivoStorage
{
    private readonly string _raiz;

    public ArchivoStorage(string raiz)
    {
        _raiz = Path.GetFullPath(raiz);
        Directory.CreateDirectory(_raiz);
    }

    public async Task<string> GuardarAsync(string carpeta, string extension, Stream contenido,
        CancellationToken ct = default)
    {
        
        var relativa = $"{carpeta}/{Guid.NewGuid():N}{extension}";
        var completa = Resolver(relativa);
        Directory.CreateDirectory(Path.GetDirectoryName(completa)!);

        await using var fs = new FileStream(completa, FileMode.CreateNew, FileAccess.Write,
            FileShare.None, 81920, useAsync: true);
        await contenido.CopyToAsync(fs, ct);
        return relativa;
    }

    public Stream? Abrir(string rutaRelativa)
    {
        var completa = Resolver(rutaRelativa);
        return File.Exists(completa)
            ? new FileStream(completa, FileMode.Open, FileAccess.Read, FileShare.Read)
            : null;
    }

    public void Eliminar(string? rutaRelativa)
    {
        if (string.IsNullOrEmpty(rutaRelativa)) return;
        var completa = Resolver(rutaRelativa);
        if (File.Exists(completa)) File.Delete(completa);
    }

    
    private string Resolver(string rutaRelativa)
    {
        var completa = Path.GetFullPath(Path.Combine(_raiz, rutaRelativa));
        if (!completa.StartsWith(_raiz, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Ruta inválida.");
        return completa;
    }
}