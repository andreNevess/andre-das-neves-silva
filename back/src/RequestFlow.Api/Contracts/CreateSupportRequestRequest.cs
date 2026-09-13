using RequestFlow.Domain.SupportRequests;

namespace RequestFlow.Api.Contracts;

/// <summary>
/// Dados para cadastrar uma solicitacao interna.
/// </summary>
public sealed record CreateSupportRequestRequest
{
    /// <summary>
    /// Titulo curto da solicitacao.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Descricao detalhada do problema ou necessidade.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Nome da pessoa solicitante.
    /// </summary>
    public string Requester { get; init; } = string.Empty;

    /// <summary>
    /// Prioridade inicial: Low, Medium ou High.
    /// </summary>
    public RequestPriority Priority { get; init; }
}
