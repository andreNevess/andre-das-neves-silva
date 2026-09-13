# RequestFlow Fullstack Challenge

Aplicação para registrar, acompanhar e concluir solicitações internas de suporte, conforme o desafio técnico de React/Next.js, ASP.NET Core e SQL Server.

## Estrutura

- `front`: Next.js + TypeScript, App Router, Vitest e React Testing Library.
- `back`: ASP.NET Core Web API em .NET 8, EF Core, SQL Server, arquitetura hexagonal com CQRS + MediatR.

## Requisitos

- Docker Desktop em execução.
- .NET SDK 8+ ou 9+ com runtime .NET 8 instalado.
- EF Core CLI para executar migrations.
- Node.js 20+.
- npm 11+.
- Visual Studio 2022 17.8+ opcional para rodar o back pela IDE.

Instale ou atualize a CLI do EF Core, caso necessário:

```powershell
dotnet tool install --global dotnet-ef --version 8.*
# se já estiver instalado:
dotnet tool update --global dotnet-ef --version 8.*
```

## Quick Start

Execute estes passos a partir da raiz do repositório.

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

4. Em outro terminal, a partir da raiz do repositório, rode o front:

```powershell
cd front
npm install
Copy-Item .env.example .env.local
npm run dev
```

Front: `http://localhost:3000`

## Banco de Dados

O banco padrão da solução é Microsoft SQL Server 2022 em Docker, definido em `back/docker-compose.yml`.

Configuração padrão usada pela API:

- Servidor: `localhost,1433`
- Banco: `RequestFlowRequestsDb`
- Usuário: `sa`
- Senha: valor de `MSSQL_SA_PASSWORD`

A senha não fica versionada. Use uma senha forte localmente e mantenha a mesma senha nos comandos do Docker, migrations e API.

Se abrir um novo terminal, configure novamente:

```powershell
$env:MSSQL_SA_PASSWORD="<SUA_SENHA_FORTE>"
```

Se precisar sobrescrever a connection string completa, use:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=RequestFlowRequestsDb;User Id=sa;Password=<SUA_SENHA_FORTE>;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

O arquivo `back/.env.example` documenta a variável esperada pelo Docker Compose. Se preferir usar arquivo `.env`, copie `back/.env.example` para `back/.env` e preencha `MSSQL_SA_PASSWORD`; esse arquivo local é ignorado pelo Git. A API não depende desse arquivo automaticamente; para a API, use variável de ambiente ou `dotnet user-secrets`.

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

Esse comando grava a senha fora do repositório, no store local de user-secrets da máquina.

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

Conteúdo esperado em `front/.env.local`:

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

Os testes de integração da API usam Testcontainers para subir um SQL Server temporário em Docker, aplicar as migrations e validar os endpoints contra o mesmo banco escolhido para a solução. Por isso, o Docker Desktop precisa estar em execução também para a suíte de testes do back.

Front-end:

```powershell
cd front
npm run lint
npm test
npm run build
npm audit
```

## Solução de Problemas

- Se o Docker informar erro de senha, confira se a senha atende aos requisitos do SQL Server: pelo menos 8 caracteres, letras maiúsculas e minúsculas, número e símbolo.
- Se alterar a senha depois que o volume já foi criado, use a senha original daquele volume ou recrie o ambiente local com `docker compose down -v` dentro da pasta `back`. Esse comando apaga os dados locais do SQL Server.
- Se a API iniciar com erro sobre `MSSQL_SA_PASSWORD`, defina a variável de ambiente no terminal atual ou configure user-secrets no projeto `RequestFlow.Api`.
- Se a migration falhar logo após `docker compose up -d`, aguarde alguns segundos e tente novamente; o SQL Server pode ainda estar inicializando.
- Se comandos `dotnet build`, `dotnet test` ou `dotnet ef` falharem informando que DLL/PDB está em uso, pare a API no Visual Studio ou encerre o processo `RequestFlow.Api` antes de executar novamente.
- Se a porta `1433` já estiver em uso, pare o outro SQL Server local ou altere a porta publicada no `docker-compose.yml` e ajuste `SqlServer:Server`.
- Se a porta `5132` estiver em uso, altere `applicationUrl` em `back/src/RequestFlow.Api/Properties/launchSettings.json` e atualize `front/.env.local`.

## Decisões Técnicas

- .NET 8 LTS foi escolhido por estar em suporte ativo e ser conservador para avaliação.
- O back separa domínio, aplicação, infraestrutura e API. Controllers apenas recebem HTTP e delegam comandos/queries.
- MediatR organiza casos de uso em comandos e queries; FluentValidation centraliza validações de entrada.
- EF Core foi escolhido conforme solicitado; migrations versionam o schema.
- SQL Server em Docker foi definido como padrão para tornar o ambiente mais reproduzível na avaliação.
- Enums são persistidos como texto para facilitar leitura do banco e reduzir ambiguidade operacional. Assim, consultas manuais exibem `Low`, `Medium`, `High`, `Open`, `InProgress` e `Completed` em vez de códigos numéricos sem contexto.
- O front usa App Router por ser o modelo atual do Next.js, com componentes de feature e cliente HTTP tipado.
- A UI evita biblioteca de estado global porque o fluxo é pequeno; estado local com efeitos abortáveis manteve o código simples e testável.
- O middleware de exceções padroniza erros conhecidos e inesperados com `ProblemDetails`; o middleware de correlação adiciona `X-Correlation-ID` para facilitar troubleshooting.
- A pasta `docs/adr` registra decisões arquiteturais curtas para facilitar a avaliação e a evolução do projeto.

## Índices e Restrições

A migration cria:

- PK em `SupportRequests.Id`.
- Checks para prioridade, status e coerência de `CompletedAtUtc`.
- Índice composto `IX_SupportRequests_Status_Priority_CreatedAtUtc`.
- Índices em `Title` e `Requester`.

O índice composto apoia os filtros por status/prioridade e a ordenação padrão por criação mais recente. Os índices em título e solicitante ajudam cenários de busca e evoluções futuras, embora buscas com `LIKE '%termo%'` possam não usar plenamente o índice em SQL Server.

As colunas `Priority` e `Status` são salvas como texto para manter o banco legível durante consultas operacionais e avaliação manual. Para evitar valores fora do domínio, a migration também cria check constraints limitando `Priority` a `Low`, `Medium` e `High`, e `Status` a `Open`, `InProgress` e `Completed`.

## Limitações Conhecidas

- Sem autenticação/autorização.
- Sem seed automático de dados.
- Sem cache dedicado no front.
- A busca textual usa `LIKE '%termo%'`, suficiente para o desafio, mas pode não aproveitar plenamente índices comuns em bases grandes.
- Sem controle de concorrência otimista com `rowversion` ou `ETag`; em edições simultâneas, a última gravação aceita pela API prevalece.
- Observabilidade ainda básica: há correlation id e health check, mas sem tracing distribuído ou métricas.
- Sem testes end-to-end de navegador.

## Melhorias com Mais Tempo

- Autenticação, perfis e auditoria de alterações.
- Evoluir a busca textual para SQL Server Full-Text Search ou outro mecanismo especializado caso o volume de dados cresça.
- Adicionar `rowversion` na entidade e retornar `409 Conflict` quando uma atualização chegar com versão desatualizada.
- Cache no front com estratégia explícita.
- Observabilidade com logs estruturados e tracing.
- Expandir CI com cobertura, relatório de testes e validação de migrações em ambiente dedicado.
- Docker Compose completo para API e front, além do SQL Server já incluído.
- Testes E2E com Playwright cobrindo fluxo real.

## Tempo e Uso de IA

Tempo aproximado utilizado nesta implementação assistida: cerca de 3,5 horas de construção iterativa e validação local.

Ferramenta de IA utilizada: OpenAI Codex/ChatGPT para leitura do enunciado, geração orientada de código, ajustes de build/testes e documentação. As decisões de arquitetura foram aplicadas de forma explícita na estrutura do projeto.
