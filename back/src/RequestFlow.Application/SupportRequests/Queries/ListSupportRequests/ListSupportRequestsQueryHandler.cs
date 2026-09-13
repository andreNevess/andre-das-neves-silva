using RequestFlow.Application.Common.Interfaces;
using RequestFlow.Application.Common.Models;
using RequestFlow.Application.SupportRequests.Dtos;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Queries.ListSupportRequests;

public sealed class ListSupportRequestsQueryHandler
    : IRequestHandler<ListSupportRequestsQuery, PagedResult<SupportRequestDto>>
{
    private readonly ISupportRequestRepository repository;

    public ListSupportRequestsQueryHandler(ISupportRequestRepository repository)
    {
        this.repository = repository;
    }

    public async Task<PagedResult<SupportRequestDto>> Handle(
        ListSupportRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new SupportRequestListFilter(
            request.Status,
            request.Priority,
            request.Search,
            request.PageNumber,
            request.PageSize);

        var page = await repository.ListAsync(filter, cancellationToken);

        return new PagedResult<SupportRequestDto>(
            page.Items.Select(SupportRequestDto.FromEntity).ToArray(),
            page.TotalItems,
            page.PageNumber,
            page.PageSize);
    }
}
