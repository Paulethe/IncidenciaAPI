using Application.Interfaces;

namespace Application.Services;

public enum ResultadoMultimedia
{
    Ok, NoEncontrado, Base64Invalido, ArchivoVacio, DemasiadoGrande, FormatoNoPermitido
}

public class IncidenciaMultimediaService
{
    private const int MaxImagen = 5 * 1024 * 1024;
    private const long MaxVideo = 30L * 1024 * 1024;
    private static readonly HashSet<string> ExtImagen =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly IIncidenciaRepository _repo;
    private readonly IArchivoStorage _storage;

    public IncidenciaMultimediaService(IIncidenciaRepository repo, IArchivoStorage storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<ResultadoMultimedia> GuardarImagenBase64Async(
        int id, string? nombreArchivo, string? base64, CancellationToken ct)
    {
        var inc = await _repo.ObtenerPorIdAsync(id, ct);
        if (inc is null) return ResultadoMultimedia.NoEncontrado;
        if (string.IsNullOrWhiteSpace(base64)) return ResultadoMultimedia.Base64Invalido;

      
        base64 = base64.Trim();
        var coma = base64.IndexOf(',');
        if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && coma >= 0)
            base64 = base64[(coma + 1)..];

      
        var padding = base64.EndsWith("==") ? 2 : base64.EndsWith('=') ? 1 : 0;
        if ((long)base64.Length * 3 / 4 - padding > MaxImagen)
            return ResultadoMultimedia.DemasiadoGrande;

        byte[] bytes;
        try { bytes = Convert.FromBase64String(base64); }
        catch (FormatException) { return ResultadoMultimedia.Base64Invalido; }

        if (bytes.Length == 0) return ResultadoMultimedia.Base64Invalido;
        if (bytes.Length > MaxImagen) return ResultadoMultimedia.DemasiadoGrande;

       
        var tipo = DetectarImagen(bytes);
        if (tipo is null || !ExtImagen.Contains(Path.GetExtension(nombreArchivo ?? "")))
            return ResultadoMultimedia.FormatoNoPermitido;

        var ext = tipo switch { "image/png" => ".png", "image/webp" => ".webp", _ => ".jpg" };
        var nueva = await _storage.GuardarAsync("imagenes", ext, new MemoryStream(bytes), ct);

        var anterior = inc.ImagenRuta;
        inc.ImagenRuta = nueva;
        inc.ImagenContentType = tipo;
        await _repo.ActualizarAsync(inc, ct);
        _storage.Eliminar(anterior);
        return ResultadoMultimedia.Ok;
    }

    public async Task<ResultadoMultimedia> GuardarVideoAsync(
        int id, Stream contenido, long longitud, string? nombreArchivo, CancellationToken ct)
    {
        var inc = await _repo.ObtenerPorIdAsync(id, ct);
        if (inc is null) return ResultadoMultimedia.NoEncontrado;
        if (longitud == 0) return ResultadoMultimedia.ArchivoVacio;
        if (longitud > MaxVideo) return ResultadoMultimedia.DemasiadoGrande;

        // MP4: extensión .mp4 y la marca "ftyp" en los bytes 4-7
        var cabecera = new byte[12];
        var leidos = await contenido.ReadAsync(cabecera, ct);
        var esMp4 = leidos >= 12 && cabecera[4] == 'f' && cabecera[5] == 't'
                    && cabecera[6] == 'y' && cabecera[7] == 'p';
        if (!esMp4 || !string.Equals(Path.GetExtension(nombreArchivo ?? ""), ".mp4",
                StringComparison.OrdinalIgnoreCase))
            return ResultadoMultimedia.FormatoNoPermitido;
        contenido.Position = 0;

        var nueva = await _storage.GuardarAsync("videos", ".mp4", contenido, ct);
        var anterior = inc.VideoRuta;
        inc.VideoRuta = nueva;
        inc.VideoContentType = "video/mp4";
        await _repo.ActualizarAsync(inc, ct);
        _storage.Eliminar(anterior);
        return ResultadoMultimedia.Ok;
    }

    public async Task<(Stream Contenido, string ContentType)?> ObtenerImagenAsync(int id, CancellationToken ct)
    {
        var inc = await _repo.ObtenerPorIdAsync(id, ct);
        if (inc?.ImagenRuta is null) return null;
        var s = _storage.Abrir(inc.ImagenRuta);
        return s is null ? null : (s, inc.ImagenContentType ?? "application/octet-stream");
    }

    public async Task<(Stream Contenido, string ContentType)?> ObtenerVideoAsync(int id, CancellationToken ct)
    {
        var inc = await _repo.ObtenerPorIdAsync(id, ct);
        if (inc?.VideoRuta is null) return null;
        var s = _storage.Abrir(inc.VideoRuta);
        return s is null ? null : (s, inc.VideoContentType ?? "video/mp4");
    }

    public async Task<bool> EliminarImagenAsync(int id, CancellationToken ct)
    {
        var inc = await _repo.ObtenerPorIdAsync(id, ct);
        if (inc?.ImagenRuta is null) return false;
        _storage.Eliminar(inc.ImagenRuta);
        inc.ImagenRuta = null;
        inc.ImagenContentType = null;
        await _repo.ActualizarAsync(inc, ct);
        return true;
    }

    private static string? DetectarImagen(byte[] b)
    {
        if (b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF) return "image/jpeg";
        if (b.Length >= 8 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47) return "image/png";
        if (b.Length >= 12 && b[0] == 'R' && b[1] == 'I' && b[2] == 'F' && b[3] == 'F'
            && b[8] == 'W' && b[9] == 'E' && b[10] == 'B' && b[11] == 'P') return "image/webp";
        return null;
    }
}