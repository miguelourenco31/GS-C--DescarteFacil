using DescarteFacil.Api.Data;
using DescarteFacil.Api.Domain;
using DescarteFacil.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DescarteFacil.Tests;

/// <summary>Testes de serviço usando SQLite em memória (sem tocar no banco real).</summary>
public sealed class ServiceTests : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly AppDbContext _db;

    public ServiceTests()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_conn).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated(); // cria o schema e aplica o HasData (3 ecopontos)
    }

    public void Dispose()
    {
        _db.Dispose();
        _conn.Dispose();
    }

    [Fact]
    public async Task Ecoponto_mais_proximo_ignora_lotado_quando_ha_alternativa()
    {
        var service = new EcopontoService(_db);

        // Perto de Pinheiros (ecoponto 2, que está com 95% => lotado)
        var resultado = await service.BuscarMaisProximosAsync(-23.5614, -46.6918, 3);

        Assert.Equal(3, resultado.Count);
        Assert.False(resultado[0].Ecoponto.EstaLotado);
        Assert.True(resultado[^1].Ecoponto.EstaLotado);
        Assert.Equal(2, resultado[^1].Ecoponto.Id);
    }

    [Fact]
    public async Task Listar_ecopontos_filtra_por_bairro()
    {
        var service = new EcopontoService(_db);

        var lista = await service.ListarAsync("Mooca");

        var unico = Assert.Single(lista);
        Assert.Equal("Mooca", unico.Bairro);
    }

    [Fact]
    public async Task Registrar_leitura_persiste_no_banco()
    {
        var service = new EcopontoService(_db);

        await service.RegistrarLeituraAsync(1, 97);

        var salvo = await service.ObterAsync(1);
        Assert.NotNull(salvo);
        Assert.Equal(97, salvo!.OcupacaoPercentual);
        Assert.True(salvo.EstaLotado);
    }

    [Fact]
    public async Task Criar_solicitacao_com_ecoponto_inexistente_deve_falhar()
    {
        var service = new SolicitacaoService(_db);
        var s = Nova("Mooca", ecopontoId: 999);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(s));
    }

    [Fact]
    public async Task Fluxo_de_solicitacao_persiste_status()
    {
        var service = new SolicitacaoService(_db);
        var criada = await service.CriarAsync(Nova("Mooca", ecopontoId: 3));

        await service.AgendarAsync(criada.Id);
        var concluida = await service.ConcluirAsync(criada.Id);

        Assert.Equal(StatusSolicitacao.Concluida, concluida!.Status);
    }

    [Fact]
    public async Task Agendar_solicitacao_inexistente_retorna_nulo()
    {
        var service = new SolicitacaoService(_db);
        Assert.Null(await service.AgendarAsync(12345));
    }

    [Fact]
    public async Task Resumo_por_bairro_agrupa_e_ordena_por_total()
    {
        var service = new SolicitacaoService(_db);
        await service.CriarAsync(Nova("Mooca"));
        await service.CriarAsync(Nova("Mooca"));
        var terceira = await service.CriarAsync(Nova("Sé"));
        await service.CancelarAsync(terceira.Id);

        var resumo = await service.ResumoPorBairroAsync();

        Assert.Equal("Mooca", resumo[0].Bairro);
        Assert.Equal(2, resumo[0].Total);
        Assert.Equal(2, resumo[0].Abertas);
        Assert.Equal(0, resumo[1].Abertas);
    }

    [Fact]
    public async Task Listar_doacoes_disponiveis_exclui_as_retiradas()
    {
        var service = new DoacaoService(_db);
        var a = await service.CriarAsync(new Doacao { TipoItem = TipoItem.Sofa, Descricao = "Sofá 3 lugares", NomeDoador = "Maria" });
        await service.CriarAsync(new Doacao { TipoItem = TipoItem.Geladeira, Descricao = "Geladeira funcionando", NomeDoador = "João" });
        await service.MarcarRetiradaAsync(a.Id);

        var disponiveis = await service.ListarAsync(apenasDisponiveis: true);

        var unica = Assert.Single(disponiveis);
        Assert.Equal(TipoItem.Geladeira, unica.TipoItem);
    }

    private static Solicitacao Nova(string bairro, int? ecopontoId = null) => new()
    {
        TipoItem = TipoItem.Sofa,
        Descricao = "Sofá velho de 2 lugares",
        Bairro = bairro,
        NomeSolicitante = "Teste",
        EcopontoId = ecopontoId
    };
}
