using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartamentoRepository _repository;

    public DepartamentosController(IDepartamentoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var departamentos = await _repository.ObtenerTodosAsync(cancellationToken);

        var resultado = departamentos.Select(d => new { d.Id, d.Nombre });
        return Ok(resultado);
    }
}