using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

public class ManejoErroresMiddleware(RequestDelegate next, ILogger<ManejoErroresMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, titulo) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflicto con los datos existentes"),
                ValidacionException => (StatusCodes.Status400BadRequest, "Datos inválidos"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(ex, "Error no controlado en {Metodo} {Ruta}", context.Request.Method, context.Request.Path);

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado. Intente nuevamente."
                    : ex.Message,
                Instance = context.Request.Path
            });
        }
    }
}