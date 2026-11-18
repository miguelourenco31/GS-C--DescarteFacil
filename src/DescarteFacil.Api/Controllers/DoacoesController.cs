using DescarteFacil.Api.Domain;
using DescarteFacil.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DescarteFacil.Api.Controllers;

[ApiController]
[Route("api/doacoes")]
[Produces("application/json")]
public class DoacoesController : ControllerBase
{
    private readonly IDoacaoService _service;

    public DoacoesController(IDoacaoService service) => _service = service;

    /// <summary>Lista doações; use disponiveis=true para ver só as ainda não retiradas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoacaoResponse>>> Listar(
        [FromQuery] bool? disponiveis, CancellationToken ct)
    {
        var lista = await _service.ListarAsync(disponiveis, ct);
        return Ok(lista.Select(DoacaoResponse.De));
    }

    /// <summary>Busca uma doação pelo id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoacaoResponse>> Obter(int id, CancellationToken ct)
    {
        var d = await _service.ObterAsync(id, ct);
        return d is null ? NotFound() : Ok(DoacaoResponse.De(d));
    }

    /// <summary>Oferece um item em bom estado para doação.</summary>
    [HttpPost]
    public async Task<ActionResult<DoacaoResponse>> Criar([FromBody] DoacaoRequest req, CancellationToken ct)
    {
        try
        {
            var criada = await _service.CriarAsync(new Doacao
            {
                TipoItem = req.TipoItem, Descricao = req.Descricao,
                NomeDoador = req.NomeDoador, EcopontoId = req.EcopontoId
            }, ct);

            return CreatedAtAction(nameof(Obter), new { id = criada.Id }, DoacaoResponse.De(criada));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Marca a doação como retirada por alguém.</summary>
    [HttpPatch("{id:int}/retirar")]
    public async Task<ActionResult<DoacaoResponse>> Retirar(int id, CancellationToken ct)
    {
        try
        {
            var d = await _service.MarcarRetiradaAsync(id, ct);
            return d is null ? NotFound() : Ok(DoacaoResponse.De(d));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
