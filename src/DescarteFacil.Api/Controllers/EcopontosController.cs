using DescarteFacil.Api.Domain;
using DescarteFacil.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DescarteFacil.Api.Controllers;

[ApiController]
[Route("api/ecopontos")]
[Produces("application/json")]
public class EcopontosController : ControllerBase
{
    private readonly IEcopontoService _service;

    public EcopontosController(IEcopontoService service) => _service = service;

    /// <summary>Lista os ecopontos, com filtro opcional por bairro.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EcopontoResponse>>> Listar(
        [FromQuery] string? bairro, CancellationToken ct)
    {
        var lista = await _service.ListarAsync(bairro, ct);
        return Ok(lista.Select(EcopontoResponse.De));
    }

    /// <summary>Busca um ecoponto pelo id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EcopontoResponse>> Obter(int id, CancellationToken ct)
    {
        var e = await _service.ObterAsync(id, ct);
        return e is null ? NotFound() : Ok(EcopontoResponse.De(e));
    }

    /// <summary>Retorna os ecopontos mais próximos da posição do cidadão (lotados ficam por último).</summary>
    [HttpGet("proximos")]
    public async Task<ActionResult<IEnumerable<EcopontoProximoResponse>>> Proximos(
        [FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] int quantidade = 3,
        CancellationToken ct = default)
    {
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            return BadRequest("Coordenadas inválidas.");

        var lista = await _service.BuscarMaisProximosAsync(latitude, longitude, quantidade, ct);
        return Ok(lista.Select(x => new EcopontoProximoResponse(
            EcopontoResponse.De(x.Ecoponto), Math.Round(x.DistanciaKm, 2))));
    }

    /// <summary>Cadastra um novo ecoponto.</summary>
    [HttpPost]
    public async Task<ActionResult<EcopontoResponse>> Criar([FromBody] EcopontoRequest req, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(new Ecoponto
        {
            Nome = req.Nome, Endereco = req.Endereco, Bairro = req.Bairro,
            Latitude = req.Latitude, Longitude = req.Longitude
        }, ct);

        return CreatedAtAction(nameof(Obter), new { id = criado.Id }, EcopontoResponse.De(criado));
    }

    /// <summary>Atualiza os dados de um ecoponto.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<EcopontoResponse>> Atualizar(
        int id, [FromBody] EcopontoRequest req, CancellationToken ct)
    {
        var atualizado = await _service.AtualizarAsync(id, new Ecoponto
        {
            Nome = req.Nome, Endereco = req.Endereco, Bairro = req.Bairro,
            Latitude = req.Latitude, Longitude = req.Longitude
        }, ct);

        return atualizado is null ? NotFound() : Ok(EcopontoResponse.De(atualizado));
    }

    /// <summary>Registra a leitura do sensor da caçamba (ocupação de 0 a 100%).</summary>
    [HttpPatch("{id:int}/leitura")]
    public async Task<ActionResult<EcopontoResponse>> RegistrarLeitura(
        int id, [FromBody] LeituraRequest req, CancellationToken ct)
    {
        var e = await _service.RegistrarLeituraAsync(id, req.OcupacaoPercentual, ct);
        return e is null ? NotFound() : Ok(EcopontoResponse.De(e));
    }

    /// <summary>Remove um ecoponto.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
        => await _service.RemoverAsync(id, ct) ? NoContent() : NotFound();
}
