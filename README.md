# Descarte Fácil — API (C# / .NET 10)

> Global Solution FIAP 2026.2 · CitizenTech · Engenharia de Software · Disciplina **C# Software Development**

## Ficha da CitizenTech

| Campo | Resposta |
|---|---|
| **Nome da CitizenTech** | Descarte Fácil |
| **Cidade** | São Paulo / SP  |
| **Cidadão beneficiado** | Morador de São Paulo que precisa descartar item volumoso (sofá, entulho, geladeira velha) e não sabe onde |
| **Problema, em uma frase** | A cidade tem 128 ecopontos, mas ainda há cerca de 1.400 pontos frequentes de descarte irregular, porque o cidadão não sabe onde descartar nem se o ecoponto está lotado |
| **Como funciona** | O cidadão escolhe o item no app; vê o ecoponto mais próximo, se está lotado (sensor na caçamba) e quando passa o Cata-Bagulho; um modelo de ML com dados do SP156 prevê onde surgirão novos pontos de descarte irregular para a prefeitura agir antes |
| **O que cada parte faz** | **API C# (este repositório):** cadastro de ecopontos, solicitações de descarte e doações, estado de lotação e busca do ecoponto mais próximo · **Mobile:** interface do cidadão · **IoT (ESP32/Wokwi):** sensor de ocupação da caçamba · **ML:** previsão de pontos de descarte irregular · **SOA, Cybersecurity, OS, Testing:** demais disciplinas |
| **Link do repositório** | *(colar o link do GitHub)* |
| **Indicador de impacto** | Nº de solicitações concluídas por bairro e % de pedidos resolvidos em ecoponto com vaga (`GET /api/solicitacoes/resumo-por-bairro`) |
| **ODS conectados** | 11, 3 e 13 (ver abaixo) |

### ODS e como o projeto contribui

- **ODS 11 — Cidades e comunidades sustentáveis (meta 11.6):** reduz o impacto ambiental das cidades ao orientar o descarte correto de resíduos e diminuir pontos de descarte irregular.
- **ODS 3 — Saúde e bem-estar:** menos entulho nas ruas reduz criadouros de vetores (como o mosquito da dengue) e riscos à saúde da vizinhança.
- **ODS 13 — Ação contra a mudança global do clima:** entulho que bloqueia bueiros agrava enchentes; prevenir o descarte irregular ajuda na drenagem urbana.

## Integrantes

- *(nome 1 — RM)*
- *(nome 2 — RM)*
- *(nome 3 — RM)*

## Contexto desta entrega

A API em .NET cadastra **ecopontos**, **solicitações de descarte** e **doações**. Por enquanto o app mobile funciona de forma independente; ele já está organizado para se conectar a esta API numa próxima etapa (CORS liberado e endpoints prontos para consumo).

## Tecnologias e conceitos aplicados

| Conceito da disciplina | Onde está no código |
|---|---|
| **POO** | Entidades com regras de negócio encapsuladas (`Domain/Entities.cs`): `Solicitacao.Agendar/Concluir/Cancelar`, `Ecoponto.RegistrarLeitura`, `Doacao.MarcarComoRetirada`; setters privados protegem o estado |
| **Interfaces** | `IEcopontoService`, `ISolicitacaoService`, `IDoacaoService` (`Domain/Interfaces.cs`) |
| **Injeção de Dependência** | Registro em `Program.cs` (`AddScoped`) e injeção por construtor em controllers e services |
| **Web API REST** | Controllers em `Controllers/`, verbos HTTP, códigos 200/201/204/400/404/409, DTOs com validação (`Dtos/Dtos.cs`) |
| **EF Core** | `Data/AppDbContext.cs` (MySQL, relacionamentos, conversões de enum, seed) |
| **LINQ** | Filtros, `GroupBy` (resumo por bairro), `OrderBy/ThenBy`, `Select`, `Take` nos services |
| **Async/Await** | Todo acesso a dados e todos os endpoints são assíncronos, com `CancellationToken` |
| **Testes unitários** | xUnit em `tests/DescarteFacil.Tests` (domínio e serviços com SQLite em memória) |
| **Documentação** | OpenAPI + **Scalar** em `/scalar/v1` |

## Estrutura

```
GS-C#-DescarteFacil/
├── src/DescarteFacil.Api/
│   ├── Controllers/     # endpoints REST
│   ├── Domain/          # entidades, enums e interfaces
│   ├── Data/            # DbContext (EF Core) e Migrations
│   ├── Services/        # regras de aplicação (LINQ + async)
│   ├── Dtos/            # contratos de entrada e saída
│   └── Program.cs
├── tests/DescarteFacil.Tests/
│   ├── DescarteFacil.Tests.csproj
│   ├── DomainTests.cs
│   ├── ServiceTests.cs 
├── DescarteFacil.slnx
└── README.md
```

## Como executar

Pré-requisitos: [.NET SDK 10](https://dotnet.microsoft.com/download), **MySQL 8** rodando localmente e a ferramenta do EF Core (`dotnet tool install --global dotnet-ef`).

**Configure a senha do MySQL** em `src/DescarteFacil.Api/appsettings.json` (campo `ConnectionStrings:Default`). O banco `descartefacil` é criado automaticamente na primeira execução.

```bash
# 1) restaurar e compilar
dotnet build

# 2) criar a migration inicial (apenas na primeira vez, se a pasta Migrations não existir)
dotnet ef migrations add Inicial --project src/DescarteFacil.Api --output-dir Data/Migrations

# 3) rodar a API (o banco MySQL e as tabelas são criados/atualizados automaticamente ao iniciar)
dotnet run --project src/DescarteFacil.Api

# 4) rodar os testes (usam SQLite em memória, não precisam do MySQL)
dotnet test
```

Com a API no ar, abra a documentação interativa em `http://localhost:<porta>/scalar/v1` (a porta aparece no terminal). Também é possível ver o contrato OpenAPI em `/openapi/v1.json`.

## Endpoints

### Ecopontos — `/api/ecopontos`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/ecopontos?bairro=` | Lista ecopontos (filtro opcional por bairro) |
| GET | `/api/ecopontos/{id}` | Busca por id |
| GET | `/api/ecopontos/proximos?latitude=&longitude=&quantidade=` | Ecopontos mais próximos (lotados por último) |
| POST | `/api/ecopontos` | Cadastra ecoponto |
| PUT | `/api/ecopontos/{id}` | Atualiza ecoponto |
| PATCH | `/api/ecopontos/{id}/leitura` | Registra leitura do sensor da caçamba (0–100%) |
| DELETE | `/api/ecopontos/{id}` | Remove ecoponto |

### Solicitações de descarte — `/api/solicitacoes`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/solicitacoes?status=` | Lista (filtro: `Aberta`, `Agendada`, `Concluida`, `Cancelada`) |
| GET | `/api/solicitacoes/{id}` | Busca por id |
| GET | `/api/solicitacoes/resumo-por-bairro` | Total e abertas por bairro (indicador de impacto / insumo do ML) |
| POST | `/api/solicitacoes` | Cria solicitação |
| PATCH | `/api/solicitacoes/{id}/agendar` | Aberta → Agendada |
| PATCH | `/api/solicitacoes/{id}/concluir` | Agendada → Concluída |
| PATCH | `/api/solicitacoes/{id}/cancelar` | Cancela (se ainda não finalizada) |

### Doações — `/api/doacoes`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/doacoes?disponiveis=true` | Lista doações (só as não retiradas, se `disponiveis=true`) |
| GET | `/api/doacoes/{id}` | Busca por id |
| POST | `/api/doacoes` | Oferece item para doação |
| PATCH | `/api/doacoes/{id}/retirar` | Marca como retirada |

### Exemplos de corpo (JSON)

```json
// POST /api/solicitacoes
{
  "tipoItem": "Sofa",
  "descricao": "Sofá velho de 3 lugares",
  "bairro": "Mooca",
  "nomeSolicitante": "Maria",
  "ecopontoId": 3
}

// PATCH /api/ecopontos/1/leitura
{ "ocupacaoPercentual": 92 }
```

Os enums aceitam o nome como texto: `Sofa`, `Entulho`, `Geladeira`, `Outro`.

## Integração com o resto da CitizenTech

- **Sensor ESP32 (Physical Computing):** envia `PATCH /api/ecopontos/{id}/leitura` com o percentual da caçamba.
- **App mobile:** consome `GET /api/ecopontos/proximos` para mostrar o ecoponto mais perto e se está lotado.
- **Machine Learning:** usa `GET /api/solicitacoes/resumo-por-bairro` como apoio, junto com os dados públicos do SP156.

## Dados

Os 3 ecopontos iniciais (seed) são **fictícios**, apenas para demonstração. Nenhum dado pessoal real é usado.

## Evidências da aplicação

Prints dos testes dos endpoints (Scalar) e do `dotnet test` ficam em `docs/evidencias/`.
