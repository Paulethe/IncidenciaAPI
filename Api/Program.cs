using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Api.Middleware;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Title = "Datos inválidos",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Revise los campos indicados e intente nuevamente.",
                Instance = context.HttpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });
    
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IArchivoStorage>(_ =>
    new ArchivoStorage(Path.Combine(builder.Environment.ContentRootPath, "uploads")));
builder.Services.AddScoped<IncidenciaMultimediaService>();

builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IIncidenciaRepository, IncidenciaRepository>();
builder.Services.AddScoped<IIncidenciaService, IncidenciaService>();

var app = builder.Build();

app.UseMiddleware<ManejoErroresMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();