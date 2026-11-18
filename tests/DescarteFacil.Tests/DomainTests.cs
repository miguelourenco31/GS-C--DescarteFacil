using DescarteFacil.Api.Domain;
using DescarteFacil.Api.Services;
using Xunit;

namespace DescarteFacil.Tests;

public class DomainTests
{
    [Theory]
    [InlineData(0, NivelLotacao.Vazio)]
    [InlineData(59, NivelLotacao.Vazio)]
    [InlineData(60, NivelLotacao.Moderado)]
    [InlineData(89, NivelLotacao.Moderado)]
    [InlineData(90, NivelLotacao.Cheio)]
    [InlineData(100, NivelLotacao.Cheio)]
    public void Lotacao_deve_refletir_percentual_do_sensor(int percentual, NivelLotacao esperado)
    {
        var e = new Ecoponto();
        e.RegistrarLeitura(percentual);

        Assert.Equal(esperado, e.Lotacao);
        Assert.Equal(esperado == NivelLotacao.Cheio, e.EstaLotado);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void RegistrarLeitura_fora_do_intervalo_deve_lancar(int percentual)
    {
        var e = new Ecoponto();
        Assert.Throws<ArgumentOutOfRangeException>(() => e.RegistrarLeitura(percentual));
    }

    [Fact]
    public void Solicitacao_segue_fluxo_Aberta_Agendada_Concluida()
    {
        var s = new Solicitacao();
        Assert.Equal(StatusSolicitacao.Aberta, s.Status);

        s.Agendar();
        Assert.Equal(StatusSolicitacao.Agendada, s.Status);

        s.Concluir();
        Assert.Equal(StatusSolicitacao.Concluida, s.Status);
    }

    [Fact]
    public void Solicitacao_aberta_nao_pode_ser_concluida_direto()
    {
        var s = new Solicitacao();
        Assert.Throws<InvalidOperationException>(() => s.Concluir());
    }

    [Fact]
    public void Solicitacao_concluida_nao_pode_ser_cancelada()
    {
        var s = new Solicitacao();
        s.Agendar();
        s.Concluir();
        Assert.Throws<InvalidOperationException>(() => s.Cancelar());
    }

    [Fact]
    public void Doacao_nao_pode_ser_retirada_duas_vezes()
    {
        var d = new Doacao();
        d.MarcarComoRetirada();
        Assert.True(d.Retirada);
        Assert.Throws<InvalidOperationException>(() => d.MarcarComoRetirada());
    }

    [Fact]
    public void Geo_distancia_entre_o_mesmo_ponto_e_zero()
    {
        Assert.Equal(0, Geo.DistanciaKm(-23.55, -46.63, -23.55, -46.63), 6);
    }

    [Fact]
    public void Geo_distancia_Se_a_Pinheiros_fica_perto_de_6km()
    {
        var km = Geo.DistanciaKm(-23.5505, -46.6333, -23.5614, -46.6918);
        Assert.InRange(km, 5.5, 7.5);
    }
}
