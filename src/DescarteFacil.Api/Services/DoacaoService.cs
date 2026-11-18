using DescarteFacil.Api.Data;
using DescarteFacil.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace DescarteFacil.Api.Services;

public class DoacaoService : IDoacaoService
{
    private readonly AppDbContext _db;

    public DoacaoService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Doacao>> ListarAsync(bool? apenasDisponiveis, CancellationToken ct = default)
    {
        var query = _db.Doacoes.AsNoTracking().AsQueryable();

        if (apenasDisponiveis == true)
            query = query.Where(d => !d.Retirada);

        return await query.OrderByDescending(d => d.CriadaEm).ToListAsync(ct);
    }

    public Task<Doacao?> ObterAsync(int id, CancellationToken ct = default)
        => _db.Doacoes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<Doacao> CriarAsync(Doacao doacao, CancellationToken ct = default)
    {
        if (doacao.EcopontoId.HasValue &&
            !await _db.Ecopontos.AnyAsync(e => e.Id == doacao.EcopontoId, ct))
            throw new ArgumentException("Ecoponto informado não existe.");

        _db.Doacoes.Add(doacao);
        await _db.SaveChangesAsync(ct);
        return doacao;
    }

    public async Task<Doacao?> MarcarRetiradaAsync(int id, CancellationToken ct = default)
    {
        var d = await _db.Doacoes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (d is null) return null;

        d.MarcarComoRetirada();
        await _db.SaveChangesAsync(ct);
        return d;
    }
}
