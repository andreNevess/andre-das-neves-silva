# ADR 0004 - Estado do front-end

## Status

Aceita.

## Contexto

O front precisa listar, filtrar, paginar, criar, visualizar, atualizar e excluir solicitacoes, exibindo estados de carregamento, vazio e erro.

## Decisao

O front usa componentes por feature, cliente HTTP tipado e estado local com efeitos abortaveis. A pesquisa usa debounce simples para reduzir chamadas durante a digitacao.

Nao foi adicionada uma biblioteca de cache global porque o escopo e pequeno e os fluxos cabem em estado local de pagina.

## Consequencias

A implementacao fica direta e facil de avaliar. Caso o produto cresca, a evolucao natural seria adotar uma estrategia explicita de cache e sincronizacao, como TanStack Query ou equivalente.
