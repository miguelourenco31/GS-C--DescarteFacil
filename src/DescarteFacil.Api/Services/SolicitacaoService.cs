using DescarteFacil.Api.Data;
using DescarteFacil.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace DescarteFacil.Api.Services;

public class SolicitacaoService : ISolicitacaoService
{
    private readonly AppDbContext _db;

    public SolicitacaoService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Solicitacao>> ListarAsync(StatusSolicitacao? status, CancellationToken ct = default)
    {
        var query = _db.Solicitacoes.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        return await query.OrderByDescending(s => s.CriadaEm).ToListAsync(ct);
    }

    public Task<Solicitacao?> ObterAsync(int id, CancellationToken ct = default)
        => _db.Solicitacoes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Solicitacao> CriarAsync(Solicitacao solicitacao, CancellationToken ct = default)
    {
        if (solicitacao.EcopontoId.HasValue &&
            !await _db.Ecopontos.AnyAsync(e => e.Id == solicitacao.EcopontoId, ct))
            throw new ArgumentException("Ecoponto informado não existe.");

        _db.Solicitacoes.Add(solicitacao);
        await _db.SaveChangesAsync(ct);
        return solicitacao;
    }

    public Task<Solicitacao?> AgendarAsync(int id, CancellationToken ct = default)
        => AplicarAsync(id, s => s.Agendar(), ct);

    public Task<Solicitacao?> ConcluirAsync(int id, CancellationToken ct = default)
        => AplicarAsync(id, s => s.Concluir(), ct);

    public Task<Solicitacao?> CancelarAsync(int id, CancellationToken ct = default)
        => AplicarAsync(id, s => s.Cancelar(), ct);

    public async Task<IReadOnlyList<ResumoBairro>> ResumoPorBairroAsync(CancellationToken ct = default)
    {
        // Base para o indicador de impacto e para o ML (onde há mais pedidos de descarte).
        var grupos = await _db.Solicitacoes
            .AsNoTracking()
            .GroupBy(s => s.Bairro)
            .Select(g => new
            {
                Bairro = g.Key,
                Total = g.Count(),
                Abertas = g.Count(s => s.Status == StatusSolicitacao.Aberta)
            })
            .ToListAsync(ct);

        return grupos
            .OrderByDescending(g => g.Total)
            .ThenBy(g => g.Bairro)
            .Select(g => new ResumoBairro(g.Bairro, g.Total, g.Abertas))
            .ToList();
    }

    private async Task<Solicitacao?> AplicarAsync(int id, Action<Solicitacao> acao, CancellationToken ct)
    {
        var s = await _db.Solicitacoes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null) return null;

        acao(s);
        await _db.SaveChangesAsync(ct);
        return s;
    }
}
