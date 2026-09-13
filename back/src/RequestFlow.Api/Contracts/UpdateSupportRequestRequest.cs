using RequestFlow.Domain.SupportRequests;

namespace RequestFlow.Api.Contracts;

/// <summary>
/// Dados para atualizar prioridade e status da solicitacao.
/// </summary>
public sealed record UpdateSupportRequestRequest
{
    /// <summary>
    /// Nova prioridade: Low, Medium ou High.
    /// </summary>
    public RequestPriority Priority { get; init; }

    /// <summary>
    /// Novo status: Open, InProgress ou Completed.
    /// </summary>
    public RequestStatus Status { get; init; }
}
