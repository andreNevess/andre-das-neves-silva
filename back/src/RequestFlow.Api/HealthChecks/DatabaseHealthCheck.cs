using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using RequestFlow.Infrastructure.Persistence;

namespace RequestFlow.Api.HealthChecks;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly SupportRequestsDbContext dbContext;

    public DatabaseHealthCheck(SupportRequestsDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect
            ? HealthCheckResult.Healthy("SQL Server connection is available.")
            : HealthCheckResult.Unhealthy("SQL Server connection is unavailable.");
    }
}
