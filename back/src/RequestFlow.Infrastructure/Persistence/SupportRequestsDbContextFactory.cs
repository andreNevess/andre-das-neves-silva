using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RequestFlow.Infrastructure.Persistence;

public sealed class SupportRequestsDbContextFactory : IDesignTimeDbContextFactory<SupportRequestsDbContext>
{
    public SupportRequestsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? BuildDefaultDockerConnectionString();

        var options = new DbContextOptionsBuilder<SupportRequestsDbContext>()
            .UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(SupportRequestsDbContext).Assembly.FullName))
            .Options;

        return new SupportRequestsDbContext(options);
    }

    private static string BuildDefaultDockerConnectionString()
    {
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "MSSQL_SA_PASSWORD was not configured. Set it before running EF migrations or provide ConnectionStrings__DefaultConnection.");
        }

        return SqlServerConnectionStringFactory.Create(
            Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "localhost,1433",
            Environment.GetEnvironmentVariable("SQLSERVER_DATABASE") ?? "RequestFlowRequestsDb",
            Environment.GetEnvironmentVariable("SQLSERVER_USER") ?? "sa",
            password);
    }
}
