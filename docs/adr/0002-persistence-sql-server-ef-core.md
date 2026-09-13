# ADR 0002 - Persistencia com SQL Server e EF Core

## Status

Aceita.

## Contexto

O desafio exige Microsoft SQL Server, modelo relacional coerente, restricoes, consultas parametrizadas, migrations ou script versionado e pelo menos um indice.

## Decisao

Foi escolhido EF Core com migrations versionadas. As consultas sao expressas em LINQ, gerando comandos parametrizados. Enums sao persistidos como texto para facilitar leitura operacional.

A tabela `SupportRequests` possui:

- chave primaria em `Id`;
- checks para valores validos de prioridade e status;
- check de coerencia entre status concluido e data de conclusao;
- indice composto para status, prioridade e criacao;
- indices auxiliares em titulo e solicitante.

## Consequencias

EF Core reduz codigo repetitivo e versiona o schema junto com a aplicacao. A busca textual atual usa `LIKE '%termo%'`, suficiente para o desafio, mas em alto volume deveria evoluir para full-text search ou outra estrategia especializada.
