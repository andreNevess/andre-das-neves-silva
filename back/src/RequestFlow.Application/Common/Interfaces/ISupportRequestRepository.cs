using RequestFlow.Application.Common.Models;
using RequestFlow.Domain.SupportRequests;

namespace RequestFlow.Application.Common.Interfaces;

public interface ISupportRequestRepository
{
    Task AddAsync(SupportRequest supportRequest, CancellationToken cancellationToken);

    Task<SupportRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<SupportRequest>> ListAsync(SupportRequestListFilter filter, CancellationToken cancellationToken);

    void Remove(SupportRequest supportRequest);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
