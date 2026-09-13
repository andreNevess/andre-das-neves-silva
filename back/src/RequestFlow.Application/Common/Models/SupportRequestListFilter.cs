using RequestFlow.Domain.SupportRequests;

namespace RequestFlow.Application.Common.Models;

public sealed record SupportRequestListFilter(
    RequestStatus? Status,
    RequestPriority? Priority,
    string? Search,
    int PageNumber,
    int PageSize);
