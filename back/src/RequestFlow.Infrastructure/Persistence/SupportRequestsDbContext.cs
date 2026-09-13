using RequestFlow.Domain.SupportRequests;
using Microsoft.EntityFrameworkCore;

namespace RequestFlow.Infrastructure.Persistence;

public sealed class SupportRequestsDbContext : DbContext
{
    public SupportRequestsDbContext(DbContextOptions<SupportRequestsDbContext> options)
        : base(options)
    {
    }

    public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportRequestsDbContext).Assembly);
    }
}
