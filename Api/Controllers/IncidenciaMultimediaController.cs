using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/incidencias/{id:int}")]
public class IncidenciaMultimediaController(IncidenciaMultimediaService servicio) : ControllerBase
{
    [HttpPut("imagen-base64")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> SubirImagen(int id, [FromBody] ImagenBase64Request request,
        CancellationToken ct)
        => Responder(id, await servicio.GuardarImagenBase64Async(
            id, request.NombreArchivo, request.ImagenBase64, ct));

    [HttpGet("imagen")]
    public async Task<IActionResult> ObtenerImagen(int id, CancellationToken ct)
    {
        var r = await servicio.ObtenerImagenAsync(id, ct);
        return r is null ? NotFound() : File(r.Value.Contenido, r.Value.ContentType);
    }

    [HttpDelete("imagen")]
    public async Task<IActionResult> EliminarImagen(int id, CancellationToken ct)
        => await servicio.EliminarImagenAsync(id, ct) ? NoContent() : NotFound();

    [HttpPut("video")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> SubirVideo(int id, IFormFile archivo, CancellationToken ct)
    {
        await using var stream = archivo.OpenReadStream();
        return Responder(id, await servicio.GuardarVideoAsync(
            id, stream, archivo.Length, archivo.FileName, ct));
    }

    [HttpGet("video")]
    public async Task<IActionResult> ObtenerVideo(int id, CancellationToken ct)
    {
        var r = await servicio.ObtenerVideoAsync(id, ct);

        return r is null
            ? NotFound()
            : File(r.Value.Contenido, r.Value.ContentType, enableRangeProcessing: true);
    }

    private IActionResult Responder(int id, ResultadoMultimedia resultado) => resultado switch
    {
        ResultadoMultimedia.Ok => NoContent(),
        ResultadoMultimedia.NoEncontrado => Problem(statusCode: 404, title: "No encontrado",
            detail: $"No existe una incidencia con id {id}."),
        ResultadoMultimedia.Base64Invalido => Problem(statusCode: 400, title: "Datos inválidos",
            detail: "El Base64 enviado no es válido."),
        ResultadoMultimedia.ArchivoVacio => Problem(statusCode: 400, title: "Datos inválidos",
            detail: "El archivo está vacío."),
        ResultadoMultimedia.DemasiadoGrande => Problem(statusCode: 413, title: "Archivo demasiado grande",
            detail: "La imagen no puede superar 5 MB y el video 30 MB."),
        ResultadoMultimedia.FormatoNoPermitido => Problem(statusCode: 415, title: "Formato no permitido",
            detail: "Imagen: JPG, PNG o WebP. Video: MP4."),
        _ => Problem(statusCode: 500)
    };
}