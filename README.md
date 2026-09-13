# RequestFlow Fullstack Challenge

Aplicacao para registrar, acompanhar e concluir solicitacoes internas de suporte, conforme o desafio tecnico de React/Next.js, ASP.NET Core e SQL Server.

## Estrutura

- `front`: Next.js + TypeScript, App Router, Vitest e React Testing Library.
- `back`: ASP.NET Core Web API em .NET 8, EF Core, SQL Server, arquitetura hexagonal com CQRS + MediatR.

## Requisitos

- Docker Desktop em execucao.
- .NET SDK 8+ ou 9+ com runtime .NET 8 instalado.
- EF Core CLI para executar migrations.
- Node.js 20+.
- npm 11+.
- Visual Studio 2022 17.8+ opcional para rodar o back pela IDE.

Instale ou atualize a CLI do EF Core, caso necessario:

```powershell
dotnet tool install --global dotnet-ef --version 8.*
# se ja estiver instalado:
dotnet tool update --global dotnet-ef --version 8.*
```

## Quick Start

Execute estes passos a partir da raiz do repositorio.

1. Suba o SQL Server em Docker:

```powershell
cd back
$env:MSSQL_SA_PASSWORD="<SUA_SENHA_FORTE>"
docker compose up -d
docker compose ps
```

2. Crie o banco e as tabelas com migrations:

```powershell
dotnet restore
dotnet ef database update --project src/RequestFlow.Infrastructure --startup-project src/RequestFlow.Api
```

3. Rode a API:

```powershell
dotnet run --project src/RequestFlow.Api --launch-profile http
```

API: `http://localhost:5132`  
Swagger: `http://localhost:5132/swagger`  
Health check: `http://localhost:5132/health`

4. Em outro terminal, a partir da raiz do repositorio, rode o front:

```powershell
cd front
npm install
Copy-Item .env.example .env.local
npm run dev
```

Front: `http://localhost:3000`

## Banco de Dados

O banco padrao da solucao e Microsoft SQL Server 2022 em Docker, definido em `back/docker-compose.yml`.

Configuracao padrao usada pela API:

- Servidor: `localhost,1433`
- Banco: `RequestFlowRequestsDb`
- Usuario: `sa`
- Senha: valor de `MSSQL_SA_PASSWORD`

A senha nao fica versionada. Use uma senha forte localmente e mantenha a mesma senha nos comandos do Docker, migrations e API.

Se abrir um novo terminal, configure novamente:

```powershell
$env:MSSQL_SA_PASSWORD="<SUA_SENHA_FORTE>"
```

Se precisar sobrescrever a connection string completa, use:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=RequestFlowRequestsDb;User Id=sa;Password=<SUA_SENHA_FORTE>;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

O arquivo `back/.env.example` documenta a variavel esperada pelo Docker Compose. Se preferir usar arquivo `.env`, copie `back/.env.example` para `back/.env` e preencha `MSSQL_SA_PASSWORD`; esse arquivo local e ignorado pelo Git. A API nao depende desse arquivo automaticamente; para a API, use variavel de ambiente ou `dotnet user-secrets`.

## Back no Visual Studio 2022

1. Garanta que o Docker Desktop esteja rodando.

2. Suba o SQL Server:

```powershell
cd back
$env:MSSQL_SA_PASSWORD="<SUA_SENHA_FORTE>"
docker compose up -d
```

3. Configure a senha para a API via user-secrets:

```powershell
dotnet user-secrets set "MSSQL_SA_PASSWORD" "<SUA_SENHA_FORTE>" --project src/RequestFlow.Api
```

Esse comando grava a senha fora do repositorio, no store local de user-secrets da maquina.

Para conferir se o valor foi salvo:

```powershell
dotnet user-secrets list --project src/RequestFlow.Api
```

4. Aplique as migrations:

```powershell
dotnet ef database update --project src/RequestFlow.Infrastructure --startup-project src/RequestFlow.Api
```

Mantenha `MSSQL_SA_PASSWORD` definido nesse terminal ao executar as migrations. Alternativamente, use `ConnectionStrings__DefaultConnection` com a connection string completa.

5. Abra `back/RequestFlow.sln` no Visual Studio.

6. Marque `RequestFlow.Api` como startup project.

7. Rode com o profile `http`.

Swagger: `http://localhost:5132/swagger`

## Front

O front espera a API em `http://localhost:5132`.

Arquivo de exemplo:

```text
front/.env.example
```

Conteudo esperado em `front/.env.local`:

```env
NEXT_PUBLIC_API_URL=http://localhost:5132
```

Rodando:

```powershell
cd front
npm install
Copy-Item .env.example .env.local
npm run dev
```

Abra `http://localhost:3000`.

## Testes e Qualidade

Back-end:

```powershell
cd back
dotnet test RequestFlow.sln
```

Os testes de integracao da API usam Testcontainers para subir um SQL Server temporario em Docker, aplicar as migrations e validar os endpoints contra o mesmo banco escolhido para a solucao. Por isso, o Docker Desktop precisa estar em execucao tambem para a suite de testes do back.

Front-end:

```powershell
cd front
npm run lint
npm test
npm run build
npm audit
```

## Solucao de Problemas

- Se o Docker informar erro de senha, confira se a senha atende aos requisitos do SQL Server: pelo menos 8 caracteres, letras maiusculas e minusculas, numero e simbolo.
- Se alterar a senha depois que o volume ja foi criado, remova o container/volume antigo ou use a senha original daquele volume.
- Se a API iniciar com erro sobre `MSSQL_SA_PASSWORD`, defina a variavel de ambiente no terminal atual ou configure user-secrets no projeto `RequestFlow.Api`.
- Se a migration falhar logo apos `docker compose up -d`, aguarde alguns segundos e tente novamente; o SQL Server pode ainda estar inicializando.
- Se a porta `1433` ja estiver em uso, pare o outro SQL Server local ou altere a porta publicada no `docker-compose.yml` e ajuste `SqlServer:Server`.
- Se a porta `5132` estiver em uso, altere `applicationUrl` em `back/src/RequestFlow.Api/Properties/launchSettings.json` e atualize `front/.env.local`.

## Decisoes Tecnicas

- .NET 8 LTS foi escolhido por estar em suporte ativo e ser conservador para avaliacao.
- O back separa dominio, aplicacao, infraestrutura e API. Controllers apenas recebem HTTP e delegam comandos/queries.
- MediatR organiza casos de uso em comandos e queries; FluentValidation centraliza validacoes de entrada.
- EF Core foi escolhido conforme solicitado; migrations versionam o schema.
- SQL Server em Docker foi definido como padrao para tornar o ambiente mais reproduzivel na avaliacao.
- Enums sao persistidos como texto para facilitar leitura do banco e reduzir ambiguidade operacional. Assim, consultas manuais exibem `Low`, `Medium`, `High`, `Open`, `InProgress` e `Completed` em vez de codigos numericos sem contexto.
- O front usa App Router por ser o modelo atual do Next.js, com componentes de feature e cliente HTTP tipado.
- A UI evita biblioteca de estado global porque o fluxo e pequeno; estado local com efeitos abortaveis manteve o codigo simples e testavel.
- O middleware de excecoes padroniza erros conhecidos e inesperados com `ProblemDetails`; o middleware de correlacao adiciona `X-Correlation-ID` para facilitar troubleshooting.
- A pasta `docs/adr` registra decisoes arquiteturais curtas para facilitar a avaliacao e a evolucao do projeto.

## Indices e Restricoes

A migration cria:

- PK em `SupportRequests.Id`.
- Checks para prioridade, status e coerencia de `CompletedAtUtc`.
- Indice composto `IX_SupportRequests_Status_Priority_CreatedAtUtc`.
- Indices em `Title` e `Requester`.

O indice composto apoia os filtros por status/prioridade e a ordenacao padrao por criacao mais recente. Os indices em titulo e solicitante ajudam cenarios de busca e evolucoes futuras, embora buscas com `LIKE '%termo%'` possam nao usar plenamente o indice em SQL Server.

As colunas `Priority` e `Status` sao salvas como texto para manter o banco legivel durante consultas operacionais e avaliacao manual. Para evitar valores fora do dominio, a migration tambem cria check constraints limitando `Priority` a `Low`, `Medium` e `High`, e `Status` a `Open`, `InProgress` e `Completed`.

## Limitacoes Conhecidas

- Sem autenticacao/autorizacao.
- Sem seed automatico de dados.
- Sem cache dedicado no front.
- A busca textual usa `LIKE '%termo%'`, suficiente para o desafio, mas pode nao aproveitar plenamente indices comuns em bases grandes.
- Sem controle de concorrencia otimista com `rowversion` ou `ETag`; em edicoes simultaneas, a ultima gravacao aceita pela API prevalece.
- Observabilidade ainda basica: ha correlation id e health check, mas sem tracing distribuido ou metricas.
- Sem testes end-to-end de navegador.

## Melhorias com Mais Tempo

- Autenticacao, perfis e auditoria de alteracoes.
- Evoluir a busca textual para SQL Server Full-Text Search ou outro mecanismo especializado caso o volume de dados cresca.
- Adicionar `rowversion` na entidade e retornar `409 Conflict` quando uma atualizacao chegar com versao desatualizada.
- Cache no front com estrategia explicita.
- Observabilidade com logs estruturados e tracing.
- Expandir CI com cobertura, relatorio de testes e validacao de migracoes em ambiente dedicado.
- Docker Compose completo para API e front, alem do SQL Server ja incluido.
- Testes E2E com Playwright cobrindo fluxo real.

## Tempo e Uso de IA

Tempo aproximado utilizado nesta implementacao assistida: cerca de 3,5 horas de construcao iterativa e validacao local.

Ferramenta de IA utilizada: OpenAI Codex/ChatGPT para leitura do enunciado, geracao orientada de codigo, ajustes de build/testes e documentacao. As decisoes de arquitetura foram aplicadas de forma explicita na estrutura do projeto.
