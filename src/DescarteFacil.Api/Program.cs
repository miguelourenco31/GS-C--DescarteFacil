using System.Text.Json.Serialization;
using DescarteFacil.Api.Data;
using DescarteFacil.Api.Domain;
using DescarteFacil.Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

// Banco MySQL; a connection string fica em appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' não encontrada.");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

// Injeção de dependência: interfaces -> implementações
builder.Services.AddScoped<IEcopontoService, EcopontoService>();
builder.Services.AddScoped<ISolicitacaoService, SolicitacaoService>();
builder.Services.AddScoped<IDoacaoService, DoacaoService>();

// CORS liberado para o app mobile (Expo) consumir a API em desenvolvimento
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Aplica as migrations ao iniciar (cria o banco e as tabelas na primeira execução)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapOpenApi();
app.MapScalarApiReference();   // documentação em /scalar/v1
app.UseCors();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

app.Run();

public partial class Program { }
