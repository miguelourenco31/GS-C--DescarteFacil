namespace DescarteFacil.Api.Domain;

public interface IEcopontoService
{
    Task<IReadOnlyList<Ecoponto>> ListarAsync(string? bairro, CancellationToken ct = default);
    Task<Ecoponto?> ObterAsync(int id, CancellationToken ct = default);
    Task<Ecoponto> CriarAsync(Ecoponto ecoponto, CancellationToken ct = default);
    Task<Ecoponto?> AtualizarAsync(int id, Ecoponto dados, CancellationToken ct = default);
    Task<bool> RemoverAsync(int id, CancellationToken ct = default);
    Task<Ecoponto?> RegistrarLeituraAsync(int id, int percentual, CancellationToken ct = default);

    /// <summary>Retorna os ecopontos mais próximos, deixando os lotados por último.</summary>
    Task<IReadOnlyList<EcopontoProximo>> BuscarMaisProximosAsync(
        double latitude, double longitude, int quantidade, CancellationToken ct = default);
}

public interface ISolicitacaoService
{
    Task<IReadOnlyList<Solicitacao>> ListarAsync(StatusSolicitacao? status, CancellationToken ct = default);
    Task<Solicitacao?> ObterAsync(int id, CancellationToken ct = default);
    Task<Solicitacao> CriarAsync(Solicitacao solicitacao, CancellationToken ct = default);
    Task<Solicitacao?> AgendarAsync(int id, CancellationToken ct = default);
    Task<Solicitacao?> ConcluirAsync(int id, CancellationToken ct = default);
    Task<Solicitacao?> CancelarAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ResumoBairro>> ResumoPorBairroAsync(CancellationToken ct = default);
}

public interface IDoacaoService
{
    Task<IReadOnlyList<Doacao>> ListarAsync(bool? apenasDisponiveis, CancellationToken ct = default);
    Task<Doacao?> ObterAsync(int id, CancellationToken ct = default);
    Task<Doacao> CriarAsync(Doacao doacao, CancellationToken ct = default);
    Task<Doacao?> MarcarRetiradaAsync(int id, CancellationToken ct = default);
}

public record EcopontoProximo(Ecoponto Ecoponto, double DistanciaKm);

public record ResumoBairro(string Bairro, int Total, int Abertas);
