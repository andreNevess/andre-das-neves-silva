# ADR 0001 - Arquitetura do back-end

## Status

Aceita.

## Contexto

O desafio pede uma API ASP.NET Core com regras de negocio fora dos controllers, persistencia em SQL Server e capacidade de explicar as decisoes tecnicas.

## Decisao

O back-end foi dividido em quatro projetos:

- `RequestFlow.Domain`: entidades, enums e regras de negocio.
- `RequestFlow.Application`: comandos, queries, DTOs, validacoes e interfaces de porta.
- `RequestFlow.Infrastructure`: EF Core, SQL Server, migrations e implementacoes de portas.
- `RequestFlow.Api`: contratos HTTP, controllers, Swagger, middlewares e composicao.

MediatR organiza os casos de uso em comandos e queries. FluentValidation centraliza validacoes de entrada.

## Consequencias

A solucao fica mais verbosa do que um CRUD simples, mas facilita testes, substituicao de infraestrutura e leitura das responsabilidades. Para um escopo maior, a mesma divisao permite adicionar novos adapters sem concentrar regra de negocio na API.
