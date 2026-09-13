using RequestFlow.Application.Common.Interfaces;
using RequestFlow.Application.Common.Models;
using RequestFlow.Domain.SupportRequests;
using Microsoft.EntityFrameworkCore;

namespace RequestFlow.Infrastructure.Persistence.Repositories;

public sealed class EfSupportRequestRepository : ISupportRequestRepository
{
    private readonly SupportRequestsDbContext dbContext;

    public EfSupportRequestRepository(SupportRequestsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(SupportRequest supportRequest, CancellationToken cancellationToken)
    {
        await dbContext.SupportRequests.AddAsync(supportRequest, cancellationToken);
    }

    public async Task<SupportRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.SupportRequests
            .FirstOrDefaultAsync(supportRequest => supportRequest.Id == id, cancellationToken);
    }

    public async Task<PagedResult<SupportRequest>> ListAsync(
        SupportRequestListFilter filter,
        CancellationToken cancellationToken)
    {
        var query = dbContext.SupportRequests.AsNoTracking().AsQueryable();

        if (filter.Status.HasValue)
        {
            query = query.Where(supportRequest => supportRequest.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(supportRequest => supportRequest.Priority == filter.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = $"%{filter.Search.Trim()}%";
            query = query.Where(supportRequest =>
                EF.Functions.Like(supportRequest.Title, search) ||
                EF.Functions.Like(supportRequest.Requester, search));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(supportRequest => supportRequest.CreatedAtUtc)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<SupportRequest>(
            items,
            totalItems,
            filter.PageNumber,
            filter.PageSize);
    }

    public void Remove(SupportRequest supportRequest)
    {
        dbContext.SupportRequests.Remove(supportRequest);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
