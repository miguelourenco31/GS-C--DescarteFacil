using DescarteFacil.Api.Domain;
using DescarteFacil.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DescarteFacil.Api.Controllers;

[ApiController]
[Route("api/solicitacoes")]
[Produces("application/json")]
public class SolicitacoesController : ControllerBase
{
    private readonly ISolicitacaoService _service;

    public SolicitacoesController(ISolicitacaoService service) => _service = service;

    /// <summary>Lista solicitações de descarte, com filtro opcional por status.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolicitacaoResponse>>> Listar(
        [FromQuery] StatusSolicitacao? status, CancellationToken ct)
    {
        var lista = await _service.ListarAsync(status, ct);
        return Ok(lista.Select(SolicitacaoResponse.De));
    }

    /// <summary>Busca uma solicitação pelo id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SolicitacaoResponse>> Obter(int id, CancellationToken ct)
    {
        var s = await _service.ObterAsync(id, ct);
        return s is null ? NotFound() : Ok(SolicitacaoResponse.De(s));
    }

    /// <summary>Resumo de solicitações por bairro (insumo para o indicador de impacto e para o ML).</summary>
    [HttpGet("resumo-por-bairro")]
    public async Task<ActionResult<IEnumerable<ResumoBairro>>> ResumoPorBairro(CancellationToken ct)
        => Ok(await _service.ResumoPorBairroAsync(ct));

    /// <summary>Cria uma solicitação de descarte (sofá, entulho, geladeira...).</summary>
    [HttpPost]
    public async Task<ActionResult<SolicitacaoResponse>> Criar(
        [FromBody] SolicitacaoRequest req, CancellationToken ct)
    {
        try
        {
            var criada = await _service.CriarAsync(new Solicitacao
            {
                TipoItem = req.TipoItem, Descricao = req.Descricao, Bairro = req.Bairro,
                NomeSolicitante = req.NomeSolicitante, EcopontoId = req.EcopontoId
            }, ct);

            return CreatedAtAction(nameof(Obter), new { id = criada.Id }, SolicitacaoResponse.De(criada));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Agenda a retirada/entrega (Aberta → Agendada).</summary>
    [HttpPatch("{id:int}/agendar")]
    public Task<ActionResult<SolicitacaoResponse>> Agendar(int id, CancellationToken ct)
        => Transicao(() => _service.AgendarAsync(id, ct));

    /// <summary>Conclui a solicitação (Agendada → Concluída).</summary>
    [HttpPatch("{id:int}/concluir")]
    public Task<ActionResult<SolicitacaoResponse>> Concluir(int id, CancellationToken ct)
        => Transicao(() => _service.ConcluirAsync(id, ct));

    /// <summary>Cancela a solicitação.</summary>
    [HttpPatch("{id:int}/cancelar")]
    public Task<ActionResult<SolicitacaoResponse>> Cancelar(int id, CancellationToken ct)
        => Transicao(() => _service.CancelarAsync(id, ct));

    private async Task<ActionResult<SolicitacaoResponse>> Transicao(Func<Task<Solicitacao?>> acao)
    {
        try
        {
            var s = await acao();
            return s is null ? NotFound() : Ok(SolicitacaoResponse.De(s));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
