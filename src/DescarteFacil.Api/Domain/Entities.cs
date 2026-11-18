namespace DescarteFacil.Api.Domain;

public enum TipoItem
{
    Sofa = 1,
    Entulho = 2,
    Geladeira = 3,
    Outro = 4
}

public enum StatusSolicitacao
{
    Aberta = 1,
    Agendada = 2,
    Concluida = 3,
    Cancelada = 4
}

public enum NivelLotacao
{
    Vazio = 0,
    Moderado = 1,
    Cheio = 2
}

/// <summary>Ponto de entrega voluntária (ecoponto) da cidade.</summary>
public class Ecoponto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>Percentual de ocupação da caçamba (0 a 100), enviado pelo sensor (ESP32).</summary>
    public int OcupacaoPercentual { get; private set; }
    public DateTime? UltimaLeituraEm { get; private set; }

    public ICollection<Solicitacao> Solicitacoes { get; set; } = new List<Solicitacao>();
    public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();

    public NivelLotacao Lotacao => OcupacaoPercentual switch
    {
        >= 90 => NivelLotacao.Cheio,
        >= 60 => NivelLotacao.Moderado,
        _ => NivelLotacao.Vazio
    };

    public bool EstaLotado => Lotacao == NivelLotacao.Cheio;

    public void RegistrarLeitura(int percentual, DateTime? quando = null)
    {
        if (percentual is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentual), "A ocupação deve estar entre 0 e 100.");

        OcupacaoPercentual = percentual;
        UltimaLeituraEm = quando ?? DateTime.UtcNow;
    }
}

/// <summary>Pedido de descarte de um item (ex.: sofá velho) feito por um cidadão.</summary>
public class Solicitacao
{
    public int Id { get; set; }
    public TipoItem TipoItem { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string NomeSolicitante { get; set; } = string.Empty;
    public StatusSolicitacao Status { get; private set; } = StatusSolicitacao.Aberta;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public int? EcopontoId { get; set; }
    public Ecoponto? Ecoponto { get; set; }

    public void Agendar()
    {
        if (Status != StatusSolicitacao.Aberta)
            throw new InvalidOperationException("Só é possível agendar uma solicitação aberta.");
        Status = StatusSolicitacao.Agendada;
    }

    public void Concluir()
    {
        if (Status != StatusSolicitacao.Agendada)
            throw new InvalidOperationException("Só é possível concluir uma solicitação agendada.");
        Status = StatusSolicitacao.Concluida;
    }

    public void Cancelar()
    {
        if (Status is StatusSolicitacao.Concluida or StatusSolicitacao.Cancelada)
            throw new InvalidOperationException("Solicitação já finalizada não pode ser cancelada.");
        Status = StatusSolicitacao.Cancelada;
    }
}

/// <summary>Oferta de doação de um item em bom estado, em vez de descartá-lo.</summary>
public class Doacao
{
    public int Id { get; set; }
    public TipoItem TipoItem { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string NomeDoador { get; set; } = string.Empty;
    public bool Retirada { get; private set; }
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public int? EcopontoId { get; set; }
    public Ecoponto? Ecoponto { get; set; }

    public void MarcarComoRetirada()
    {
        if (Retirada)
            throw new InvalidOperationException("Doação já foi retirada.");
        Retirada = true;
    }
}
