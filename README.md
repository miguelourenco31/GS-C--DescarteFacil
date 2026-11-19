# Descarte Fácil — API C# / .NET 10

**Global Solution FIAP 2026.2 · CitizenTech · Engenharia de Software · C# Software Development**

API REST desenvolvida em C# e ASP.NET Core para apoiar o descarte correto de resíduos volumosos na cidade de São Paulo, permitindo o gerenciamento de ecopontos, solicitações de descarte, doações e informações sobre a ocupação das caçambas.

## 1. Ficha da CitizenTech

| Campo | Descrição |
|---|---|
| **Nome da CitizenTech** | Descarte Fácil |
| **Cidade** | São Paulo — SP |
| **Cidadão beneficiado** | Moradores de São Paulo que precisam descartar itens volumosos, como sofás, entulho e geladeiras, e não sabem onde realizar o descarte adequado |
| **Problema identificado** | Apesar da existência de 128 ecopontos municipais, São Paulo apresenta cerca de 1.400 pontos recorrentes de descarte irregular, evidenciando dificuldades de informação e acesso aos serviços de descarte |
| **Solução proposta** | Uma plataforma que orienta o cidadão sobre ecopontos, ocupação de caçambas, serviços de coleta e alternativas de doação, com possibilidade de apoiar ações preventivas da prefeitura por meio de análise de dados |
| **API C#** | Gerenciamento de ecopontos, solicitações, doações, leituras de ocupação e busca de ecopontos próximos |
| **Mobile** | Interface destinada ao cidadão |
| **IoT** | Simulação de monitoramento de ocupação das caçambas utilizando ESP32 e Wokwi |
| **Machine Learning** | Proposta de previsão de áreas com maior risco de descarte irregular a partir de dados públicos |
| **Repositório** | https://github.com/miguelourenco31/GS-C--DescarteFacil.git |
| **Indicadores disponíveis na API** | Total de solicitações e quantidade de solicitações abertas por bairro |
| **ODS relacionados** | ODS 11, ODS 3 e ODS 13 |

### Objetivos de Desenvolvimento Sustentável

**ODS 11 — Cidades e comunidades sustentáveis**

Contribui para a meta 11.6 ao incentivar o descarte correto de resíduos, facilitar a localização de ecopontos e apoiar a redução de pontos de descarte irregular.

**ODS 3 — Saúde e bem-estar**

O descarte adequado de resíduos pode contribuir para a redução de criadouros de vetores, obstruções de vias e riscos sanitários relacionados ao acúmulo de materiais descartados irregularmente.

**ODS 13 — Ação contra a mudança global do clima**

A iniciativa contribui indiretamente para a resiliência urbana ao incentivar práticas que reduzem o descarte irregular e o acúmulo de materiais capazes de prejudicar a drenagem urbana.

## 2. Integrantes

| Integrante | RM |
|---|---|
| Lorenzzo Vendruscolo Dias | 558305 |
| Gabriel Martins Vannucci | 556883 |
| Miguel Marques Lourenço | 555426 |
| Pedro Henrique Ferronato | 554757 |

**Curso:** Engenharia de Software — FIAP  
**Período:** 2026.2  
**Disciplina:** C# Software Development

## 3. Contexto da entrega

Este repositório contém o backend desenvolvido para a disciplina de C# Software Development da Global Solution FIAP 2026.2.

A aplicação foi construída utilizando ASP.NET Core Web API, Entity Framework Core e MySQL, com uma arquitetura organizada em entidades de domínio, interfaces, serviços, controladores e DTOs.

A API oferece operações para:

- Cadastrar, consultar, atualizar e remover ecopontos.
- Consultar ecopontos próximos de uma localização geográfica.
- Registrar leituras de ocupação das caçambas.
- Criar e acompanhar solicitações de descarte.
- Gerenciar os estados das solicitações.
- Cadastrar e consultar ofertas de doação.
- Registrar a retirada de itens doados.
- Consultar indicadores de solicitações por bairro.

O aplicativo Mobile, o módulo IoT e o componente de Machine Learning fazem parte da proposta geral da CitizenTech, mas são desenvolvidos separadamente. A integração direta desses módulos com esta API está prevista para uma etapa posterior.

## 4. Tecnologias utilizadas

- **C# e .NET 10:** linguagem e plataforma de desenvolvimento.
- **ASP.NET Core Web API:** construção dos endpoints REST.
- **Entity Framework Core:** mapeamento objeto-relacional e persistência.
- **MySQL 8:** banco de dados relacional.
- **LINQ:** consultas, filtros, ordenações e agrupamentos.
- **Async/Await:** operações assíncronas de acesso aos dados.
- **Injeção de dependência:** desacoplamento entre interfaces e implementações.
- **xUnit:** testes automatizados.
- **SQLite em memória:** suporte aos testes de serviços.
- **OpenAPI e Scalar:** documentação interativa dos endpoints.
- **Git e GitHub:** versionamento e disponibilização do código-fonte.

## 5. Conceitos de C# aplicados

| Conceito | Aplicação no projeto |
|---|---|
| **Programação Orientada a Objetos** | Entidades `Ecoponto`, `Solicitacao` e `Doacao`, com atributos, métodos e regras de negócio |
| **Encapsulamento** | Propriedades com setters privados e alterações de estado realizadas por métodos específicos |
| **Interfaces** | `IEcopontoService`, `ISolicitacaoService` e `IDoacaoService`, em `Domain/Interfaces.cs` |
| **Injeção de dependência** | Registro de serviços no `Program.cs` e utilização de interfaces nos componentes da aplicação |
| **API REST** | Controllers, verbos HTTP, rotas, DTOs e códigos de resposta |
| **Entity Framework Core** | `AppDbContext`, relacionamentos, mapeamentos e migrations |
| **LINQ** | Filtros por bairro e status, ordenação, agrupamento e seleção de resultados |
| **Async/Await** | Métodos assíncronos com `Task`, `Task<T>` e `CancellationToken` |
| **Tratamento de regras de negócio** | Validação de transições de status, limites de ocupação e retirada de doações |
| **Testes automatizados** | Testes de domínio e serviços utilizando xUnit |
| **Documentação de API** | OpenAPI com interface Scalar |

### Exemplos de regras de negócio

**Ecoponto**

O método `RegistrarLeitura()` recebe o percentual de ocupação da caçamba e valida valores entre 0 e 100.

A classificação de lotação é calculada conforme o percentual registrado:

| Ocupação | Classificação |
|---|---|
| Abaixo de 60% | Vazio |
| De 60% a 89% | Moderado |
| A partir de 90% | Cheio |

**Solicitação**

Uma solicitação pode passar pelos estados:

`Aberta → Agendada → Concluída`

Também pode ser cancelada enquanto não estiver finalizada, respeitando as validações do domínio.

**Doação**

Uma doação é inicialmente registrada como não retirada. O método `MarcarComoRetirada()` altera seu estado e impede que a operação seja repetida.

## 6. Estrutura do projeto

```text
GS-C--DescarteFacil/
├── src/
│   └── DescarteFacil.Api/
│       ├── Controllers/
│       ├── Domain/
│       │   ├── Entities.cs
│       │   └── Interfaces.cs
│       ├── Data/
│       │   └── AppDbContext.cs
│       ├── Dtos/
│       ├── Migrations/
│       ├── Services/
│       ├── Program.cs
│       └── appsettings.json
├── tests/
│   └── DescarteFacil.Tests/
│       ├── DomainTests.cs
│       ├── ServiceTests.cs
│       └── DescarteFacil.Tests.csproj
├── docs/
│   └── evidencias/
├── DescarteFacil.slnx
└── README.md
```

### Organização das camadas

**Domain:** contém entidades, enumerações, interfaces e regras de negócio.

**Controllers:** recebem as requisições HTTP, validam os dados de entrada e retornam as respostas da API.

**Services:** implementam as operações de negócio e as consultas ao banco de dados.

**Data:** contém o contexto do Entity Framework Core e suas configurações.

**Migrations:** mantém o histórico das alterações da estrutura do banco de dados.

**Dtos:** define os contratos de entrada e saída utilizados pela API.

**Tests:** contém testes automatizados para validação do domínio e dos serviços.

## 7. Como executar o projeto

### Pré-requisitos

É necessário possuir:

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- MySQL 8 instalado e em execução
- Git
- Ferramenta `dotnet-ef`

Para instalar a ferramenta do Entity Framework Core, caso ainda não esteja disponível:

```bash
dotnet tool install --global dotnet-ef
```

Caso a ferramenta já esteja instalada, esse passo pode ser ignorado.

### Passo 1 — Clonar o repositório

```bash
git clone https://github.com/miguelourenco31/GS-C--DescarteFacil.git
cd GS-C--DescarteFacil
```

### Passo 2 — Configurar o MySQL

Certifique-se de que o serviço MySQL esteja em execução.

Configure a connection string `ConnectionStrings:Default` no arquivo:

```text
src/DescarteFacil.Api/appsettings.json
```

A configuração deve utilizar o servidor, usuário e senha correspondentes à instalação local do MySQL.

Exemplo de formato:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=descartefacil;User=root;Password=SUA_SENHA_LOCAL;"
  }
}
```

O exemplo acima é ilustrativo. Preserve as demais configurações existentes no arquivo original.

**Importante:** não publique credenciais reais no GitHub. Para manter a senha fora dos arquivos versionados, é possível utilizar uma variável de ambiente local, por exemplo:

```powershell
$env:ConnectionStrings__Default = "Server=localhost;Port=3306;Database=descartefacil;User=root;Password=SUA_SENHA_LOCAL;"
```

Essa variável deve ser configurada no terminal utilizado para executar a API.

### Passo 3 — Restaurar e compilar

Na raiz do projeto:

```bash
dotnet restore
dotnet build
```

O comando `dotnet build` compila a solução e verifica possíveis erros de compilação.

### Passo 4 — Aplicar as migrations

As migrations já estão incluídas no repositório.

Não é necessário gerar uma nova migration para executar o projeto.

Execute:

```bash
dotnet ef database update --project src/DescarteFacil.Api --startup-project src/DescarteFacil.Api
```

Esse comando aplica as migrations existentes ao MySQL configurado, criando ou atualizando a estrutura do banco de dados conforme necessário.

### Passo 5 — Iniciar a API

```bash
dotnet run --project src/DescarteFacil.Api
```

No ambiente utilizado para desenvolvimento e testes, a API foi executada em:

```text
http://localhost:5000
```

Caso o terminal informe outra porta, utilize o endereço exibido na inicialização.

### Passo 6 — Acessar a documentação

Com a aplicação em execução, acesse:

**Scalar:**

http://localhost:5000/scalar/v1

**Documento OpenAPI:**

http://localhost:5000/openapi/v1.json

A interface Scalar permite visualizar os endpoints, preencher parâmetros, enviar requisições e consultar os códigos de resposta.

### Passo 7 — Executar os testes automatizados

Em outro terminal, na raiz do projeto:

```bash
dotnet test
```

Os testes utilizam xUnit e SQLite em memória, não exigindo conexão com o banco MySQL utilizado pela API.

**Resultado obtido durante a validação do projeto: 22 testes aprovados.**

## 8. Endpoints da API

### Ecopontos — `/api/ecopontos`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/ecopontos` | Lista os ecopontos cadastrados |
| GET | `/api/ecopontos?bairro=Vila%20Mariana` | Filtra ecopontos por bairro |
| GET | `/api/ecopontos/{id}` | Consulta um ecoponto pelo identificador |
| GET | `/api/ecopontos/proximos?latitude=&longitude=&quantidade=` | Busca ecopontos próximos, priorizando os não lotados |
| POST | `/api/ecopontos` | Cadastra um ecoponto |
| PUT | `/api/ecopontos/{id}` | Atualiza os dados de um ecoponto |
| PATCH | `/api/ecopontos/{id}/leitura` | Registra o percentual de ocupação da caçamba |
| DELETE | `/api/ecopontos/{id}` | Remove um ecoponto |

### Solicitações — `/api/solicitacoes`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/solicitacoes` | Lista as solicitações |
| GET | `/api/solicitacoes?status=Aberta` | Filtra solicitações por status |
| GET | `/api/solicitacoes/{id}` | Consulta uma solicitação |
| GET | `/api/solicitacoes/resumo-por-bairro` | Retorna total de solicitações e quantidade de abertas por bairro |
| POST | `/api/solicitacoes` | Cadastra uma solicitação |
| PATCH | `/api/solicitacoes/{id}/agendar` | Altera o status para agendada |
| PATCH | `/api/solicitacoes/{id}/concluir` | Conclui uma solicitação agendada |
| PATCH | `/api/solicitacoes/{id}/cancelar` | Cancela uma solicitação permitida pelas regras de negócio |

### Doações — `/api/doacoes`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/doacoes` | Lista as doações cadastradas |
| GET | `/api/doacoes?disponiveis=true` | Lista apenas as doações disponíveis |
| GET | `/api/doacoes/{id}` | Consulta uma doação |
| POST | `/api/doacoes` | Cadastra uma doação |
| PATCH | `/api/doacoes/{id}/retirar` | Marca uma doação como retirada |

## 9. Exemplos de requisições

Os exemplos abaixo utilizam valores compatíveis com os testes realizados na API.

### Criar solicitação de descarte

**POST** `/api/solicitacoes`

```json
{
  "tipoItem": 1,
  "descricao": "Sofá de três lugares para descarte",
  "bairro": "Vila Mariana",
  "nomeSolicitante": "Teste FIAP",
  "ecopontoId": 4
}
```

O campo `ecopontoId` deve corresponder a um ecoponto existente no banco de dados.

### Criar uma doação

**POST** `/api/doacoes`

```json
{
  "tipoItem": 1,
  "descricao": "Sofá de dois lugares em bom estado para doação",
  "nomeDoador": "Teste FIAP",
  "ecopontoId": 4
}
```

### Registrar leitura de ocupação

**PATCH** `/api/ecopontos/1/leitura`

```json
{
  "ocupacaoPercentual": 92
}
```

A leitura representa 92% de ocupação e corresponde à classificação `Cheio` na regra de negócio da entidade.

### Enumeração de tipos de item

| Valor | Tipo |
|---|---|
| 1 | Sofa |
| 2 | Entulho |
| 3 | Geladeira |
| 4 | Outro |

Os exemplos utilizam os valores numéricos definidos na enumeração `TipoItem`.

## 10. Persistência e banco de dados

O projeto utiliza Entity Framework Core com MySQL para persistência das entidades.

O contexto `AppDbContext` é responsável pelo mapeamento das tabelas, relacionamentos e configurações de persistência.

As migrations permitem reproduzir a estrutura do banco em outro ambiente, sem a necessidade de criar manualmente todas as tabelas.

As principais entidades persistidas são:

- `Ecoponto`
- `Solicitacao`
- `Doacao`

As solicitações e doações podem possuir referência a um ecoponto por meio de `EcopontoId`.

O projeto também utiliza dados iniciais de demonstração para ecopontos.

**Os três ecopontos iniciais são fictícios e utilizados exclusivamente para testes.**

## 11. Testes automatizados

Os testes foram desenvolvidos utilizando xUnit.

A suíte contempla validações relacionadas às entidades de domínio e aos serviços da aplicação, incluindo regras de negócio e operações de persistência em ambiente de teste.

Os testes de serviços utilizam SQLite em memória, permitindo validar comportamentos sem depender do servidor MySQL local.

Para executar:

```bash
dotnet test
```

**Resultado registrado durante os testes: 22 testes aprovados.**

Além dos testes automatizados, os endpoints foram exercitados por meio do Scalar, com operações de cadastro, consulta e atualização de estado.

## 12. Integração com os demais módulos da CitizenTech

O Descarte Fácil foi concebido como uma solução multidisciplinar.

### Mobile

O aplicativo apresenta funcionalidades voltadas ao cidadão, como consulta de ecopontos, informações de coleta e doações.

A API disponibiliza endpoints que poderão ser utilizados para substituir dados simulados por informações persistidas no backend.

### IoT — ESP32 / Wokwi

O módulo IoT simula o monitoramento da ocupação de caçambas.

A API disponibiliza o endpoint:

`PATCH /api/ecopontos/{id}/leitura`

Esse endpoint permite registrar o percentual de ocupação e calcular a classificação de lotação.

### Machine Learning

O componente de Machine Learning da proposta busca apoiar a identificação de áreas com maior risco de descarte irregular.

A API disponibiliza:

`GET /api/solicitacoes/resumo-por-bairro`

O endpoint retorna indicadores de solicitações por bairro, que poderão ser utilizados como dados complementares em futuras integrações.

**Estado das integrações:** os módulos são desenvolvidos separadamente. A comunicação direta entre Mobile, IoT, Machine Learning e esta API está prevista como evolução da solução, não sendo apresentada como integração concluída nesta entrega.

## 13. Indicadores de impacto

O projeto pretende contribuir para o acompanhamento e a redução do descarte irregular na cidade.

Nesta versão, a API disponibiliza indicadores por bairro, incluindo:

- Quantidade total de solicitações cadastradas.
- Quantidade de solicitações com status `Aberta`.
- Distribuição das solicitações entre os bairros registrados.

Esses indicadores podem auxiliar no acompanhamento da demanda por serviços de descarte e fornecer informações para análises futuras.

A medição efetiva da redução do descarte irregular depende de dados operacionais e de acompanhamento posteriores à implementação da solução.

## 14. Evidências da aplicação

As evidências de execução e validação estão organizadas no diretório:

`docs/evidencias/`

Foram registrados testes de compilação, testes automatizados e operações realizadas na documentação interativa Scalar.

Entre as operações verificadas estão:

- Compilação da solução.
- Execução dos 22 testes automatizados.
- Inicialização e documentação da API.
- Consulta e cadastro de ecopontos.
- Filtro de ecopontos por bairro.
- Cadastro e consulta de solicitações.
- Agendamento de solicitações.
- Cadastro e consulta de doações.
- Registro de retirada de doações.
- Criação e aplicação das migrations no MySQL.

## 15. Considerações finais

O Descarte Fácil demonstra a aplicação prática dos conceitos de C# Software Development em uma solução voltada a um problema urbano real.

O backend utiliza programação orientada a objetos, interfaces, encapsulamento, injeção de dependência, Entity Framework Core, LINQ, programação assíncrona, API REST e testes automatizados.

A arquitetura foi organizada para permitir a evolução do sistema e sua futura integração com os demais módulos da CitizenTech.

A proposta busca facilitar o descarte responsável, ampliar o acesso à informação e contribuir para uma cidade mais limpa, sustentável e organizada.

---

**FIAP — Global Solution 2026.2**  
**Engenharia de Software · C# Software Development**  
**Projeto: Descarte Fácil — CitizenTech**
