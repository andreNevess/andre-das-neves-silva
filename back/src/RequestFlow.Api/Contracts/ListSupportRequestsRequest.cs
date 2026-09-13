using RequestFlow.Domain.SupportRequests;

namespace RequestFlow.Api.Contracts;

/// <summary>
/// Filtros de consulta para listagem de solicitacoes.
/// </summary>
public sealed record ListSupportRequestsRequest
{
    /// <summary>
    /// Status opcional: Open, InProgress ou Completed.
    /// </summary>
    public RequestStatus? Status { get; init; }

    /// <summary>
    /// Prioridade opcional: Low, Medium ou High.
    /// </summary>
    public RequestPriority? Priority { get; init; }

    /// <summary>
    /// Texto pesquisado no titulo ou no solicitante.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Numero da pagina, iniciado em 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Quantidade de registros por pagina. Padrao: 5.
    /// </summary>
    public int PageSize { get; init; } = 5;
}
