using RequestFlow.Application.Common.Models;
using RequestFlow.Application.SupportRequests.Dtos;
using RequestFlow.Domain.SupportRequests;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Queries.ListSupportRequests;

public sealed record ListSupportRequestsQuery(
    RequestStatus? Status,
    RequestPriority? Priority,
    string? Search,
    int PageNumber,
    int PageSize) : IRequest<PagedResult<SupportRequestDto>>;
