using RequestFlow.Application.Common.Interfaces;
using RequestFlow.Infrastructure.Persistence;
using RequestFlow.Infrastructure.Persistence.Repositories;
using RequestFlow.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RequestFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<SupportRequestsDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(SupportRequestsDbContext).Assembly.FullName));
        });

        services.AddScoped<ISupportRequestRepository, EfSupportRequestRepository>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
