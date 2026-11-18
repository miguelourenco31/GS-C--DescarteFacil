using System.ComponentModel.DataAnnotations;
using DescarteFacil.Api.Domain;

namespace DescarteFacil.Api.Dtos;

// ---------- Ecopontos ----------
public record EcopontoRequest(
    [Required, StringLength(120)] string Nome,
    [Required, StringLength(200)] string Endereco,
    [Required, StringLength(80)] string Bairro,
    [Range(-90, 90)] double Latitude,
    [Range(-180, 180)] double Longitude);

public record LeituraRequest([Range(0, 100)] int OcupacaoPercentual);

public record EcopontoResponse(
    int Id, string Nome, string Endereco, string Bairro,
    double Latitude, double Longitude,
    int OcupacaoPercentual, string Lotacao, bool Lotado, DateTime? UltimaLeituraEm)
{
    public static EcopontoResponse De(Ecoponto e) => new(
        e.Id, e.Nome, e.Endereco, e.Bairro, e.Latitude, e.Longitude,
        e.OcupacaoPercentual, e.Lotacao.ToString(), e.EstaLotado, e.UltimaLeituraEm);
}

public record EcopontoProximoResponse(EcopontoResponse Ecoponto, double DistanciaKm);

// ---------- Solicitações ----------
public record SolicitacaoRequest(
    [Required] TipoItem TipoItem,
    [Required, StringLength(300)] string Descricao,
    [Required, StringLength(80)] string Bairro,
    [Required, StringLength(100)] string NomeSolicitante,
    int? EcopontoId);

public record SolicitacaoResponse(
    int Id, string TipoItem, string Descricao, string Bairro, string NomeSolicitante,
    string Status, DateTime CriadaEm, int? EcopontoId)
{
    public static SolicitacaoResponse De(Solicitacao s) => new(
        s.Id, s.TipoItem.ToString(), s.Descricao, s.Bairro, s.NomeSolicitante,
        s.Status.ToString(), s.CriadaEm, s.EcopontoId);
}

// ---------- Doações ----------
public record DoacaoRequest(
    [Required] TipoItem TipoItem,
    [Required, StringLength(300)] string Descricao,
    [Required, StringLength(100)] string NomeDoador,
    int? EcopontoId);

public record DoacaoResponse(
    int Id, string TipoItem, string Descricao, string NomeDoador,
    bool Retirada, DateTime CriadaEm, int? EcopontoId)
{
    public static DoacaoResponse De(Doacao d) => new(
        d.Id, d.TipoItem.ToString(), d.Descricao, d.NomeDoador,
        d.Retirada, d.CriadaEm, d.EcopontoId);
}
