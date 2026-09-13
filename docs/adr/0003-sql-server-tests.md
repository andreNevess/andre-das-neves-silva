# ADR 0003 - Testes de integracao com SQL Server

## Status

Aceita.

## Contexto

O banco oficial da solucao e SQL Server. Um provider diferente em testes pode mascarar diferencas de comportamento em migrations, constraints e tipos SQL.

## Decisao

Os testes de integracao da API usam Testcontainers para subir um SQL Server temporario em Docker. Antes de cada cenario, o banco de teste e recriado e as migrations sao aplicadas.

## Consequencias

Os testes ficam mais fieis ao ambiente real e validam o caminho completo HTTP + MediatR + EF Core + SQL Server. Em troca, a suite de integracao passa a exigir Docker em execucao e fica mais lenta do que testes em memoria.
