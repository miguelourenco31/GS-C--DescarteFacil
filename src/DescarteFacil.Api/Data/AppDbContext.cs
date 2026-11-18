using DescarteFacil.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace DescarteFacil.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ecoponto> Ecopontos => Set<Ecoponto>();
    public DbSet<Solicitacao> Solicitacoes => Set<Solicitacao>();
    public DbSet<Doacao> Doacoes => Set<Doacao>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Ecoponto>(e =>
        {
            e.ToTable("Ecopontos");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(120);
            e.Property(x => x.Endereco).IsRequired().HasMaxLength(200);
            e.Property(x => x.Bairro).IsRequired().HasMaxLength(80);
            e.Ignore(x => x.Lotacao);
            e.Ignore(x => x.EstaLotado);
            e.HasIndex(x => x.Bairro);
        });

        b.Entity<Solicitacao>(e =>
        {
            e.ToTable("Solicitacoes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Descricao).IsRequired().HasMaxLength(300);
            e.Property(x => x.Bairro).IsRequired().HasMaxLength(80);
            e.Property(x => x.NomeSolicitante).IsRequired().HasMaxLength(100);
            e.Property(x => x.TipoItem).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.Ecoponto)
                .WithMany(x => x.Solicitacoes)
                .HasForeignKey(x => x.EcopontoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<Doacao>(e =>
        {
            e.ToTable("Doacoes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Descricao).IsRequired().HasMaxLength(300);
            e.Property(x => x.NomeDoador).IsRequired().HasMaxLength(100);
            e.Property(x => x.TipoItem).HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.Ecoponto)
                .WithMany(x => x.Doacoes)
                .HasForeignKey(x => x.EcopontoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Dados iniciais (fictícios/aproximados, apenas para demonstração)
        b.Entity<Ecoponto>().HasData(
            new { Id = 1, Nome = "Ecoponto Exemplo Sé", Endereco = "Rua Exemplo, 100", Bairro = "Sé", Latitude = -23.5505, Longitude = -46.6333, OcupacaoPercentual = 20 },
            new { Id = 2, Nome = "Ecoponto Exemplo Pinheiros", Endereco = "Av. Exemplo, 200", Bairro = "Pinheiros", Latitude = -23.5614, Longitude = -46.6918, OcupacaoPercentual = 95 },
            new { Id = 3, Nome = "Ecoponto Exemplo Mooca", Endereco = "Rua Exemplo, 300", Bairro = "Mooca", Latitude = -23.5580, Longitude = -46.5990, OcupacaoPercentual = 55 }
        );
    }
}
