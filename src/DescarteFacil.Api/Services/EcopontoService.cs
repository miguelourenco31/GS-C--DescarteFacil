using DescarteFacil.Api.Data;
using DescarteFacil.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace DescarteFacil.Api.Services;

public class EcopontoService : IEcopontoService
{
    private readonly AppDbContext _db;

    public EcopontoService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Ecoponto>> ListarAsync(string? bairro, CancellationToken ct = default)
    {
        var query = _db.Ecopontos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(bairro))
            query = query.Where(e => e.Bairro == bairro);

        return await query.OrderBy(e => e.Nome).ToListAsync(ct);
    }

    public Task<Ecoponto?> ObterAsync(int id, CancellationToken ct = default)
        => _db.Ecopontos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Ecoponto> CriarAsync(Ecoponto ecoponto, CancellationToken ct = default)
    {
        _db.Ecopontos.Add(ecoponto);
        await _db.SaveChangesAsync(ct);
        return ecoponto;
    }

    public async Task<Ecoponto?> AtualizarAsync(int id, Ecoponto dados, CancellationToken ct = default)
    {
        var existente = await _db.Ecopontos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (existente is null) return null;

        existente.Nome = dados.Nome;
        existente.Endereco = dados.Endereco;
        existente.Bairro = dados.Bairro;
        existente.Latitude = dados.Latitude;
        existente.Longitude = dados.Longitude;

        await _db.SaveChangesAsync(ct);
        return existente;
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct = default)
    {
        var existente = await _db.Ecopontos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (existente is null) return false;

        _db.Ecopontos.Remove(existente);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Ecoponto?> RegistrarLeituraAsync(int id, int percentual, CancellationToken ct = default)
    {
        var existente = await _db.Ecopontos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (existente is null) return null;

        existente.RegistrarLeitura(percentual);
        await _db.SaveChangesAsync(ct);
        return existente;
    }

    public async Task<IReadOnlyList<EcopontoProximo>> BuscarMaisProximosAsync(
        double latitude, double longitude, int quantidade, CancellationToken ct = default)
    {
        var todos = await _db.Ecopontos.AsNoTracking().ToListAsync(ct);

        // Distância calculada em memória (Haversine); poucos ecopontos por cidade.
        return todos
            .Select(e => new EcopontoProximo(e, Geo.DistanciaKm(latitude, longitude, e.Latitude, e.Longitude)))
            .OrderBy(x => x.Ecoponto.EstaLotado)   // lotados vão para o fim
            .ThenBy(x => x.DistanciaKm)
            .Take(Math.Max(1, quantidade))
            .ToList();
    }
}

public static class Geo
{
    public static double DistanciaKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double raioTerraKm = 6371.0;
        static double Rad(double g) => g * Math.PI / 180.0;

        var dLat = Rad(lat2 - lat1);
        var dLon = Rad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return raioTerraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
