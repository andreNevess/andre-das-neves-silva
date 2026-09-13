using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using RequestFlow.Api.Contracts;
using RequestFlow.Application.Common.Models;
using RequestFlow.Application.SupportRequests.Dtos;
using RequestFlow.Domain.SupportRequests;
using RequestFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace RequestFlow.Api.Tests.SupportRequests;

public sealed class SupportRequestsApiTests : IClassFixture<SupportRequestsApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient client;
    private readonly SupportRequestsApiFactory factory;

    public SupportRequestsApiTests(SupportRequestsApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateRequest_WhenPayloadIsValid_ShouldReturnCreatedRequest()
    {
        await factory.ResetDatabaseAsync();

        var request = new CreateSupportRequestRequest
        {
            Title = "Ajuste de permissao",
            Description = "Liberar acesso ao painel financeiro.",
            Requester = "Daniel Rocha",
            Priority = RequestPriority.High
        };

        var response = await client.PostAsJsonAsync("/api/requests", request, JsonOptions);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(response.StatusCode == HttpStatusCode.Created, responseBody);

        var created = await response.Content.ReadFromJsonAsync<SupportRequestDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(request.Title, created!.Title);
        Assert.Equal(RequestStatus.Open, created.Status);
        Assert.Null(created.CompletedAtUtc);

        var persisted = await client.GetAsync($"/api/requests/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, persisted.StatusCode);
    }

    [Fact]
    public async Task ListRequests_WhenFiltersSearchAndPaginationAreProvided_ShouldReturnExpectedPage()
    {
        await factory.ResetDatabaseAsync();

        var olderMatch = SupportRequest.Create(
            "VPN sem acesso",
            "Usuario nao acessa a rede corporativa.",
            "Ana Silva",
            RequestPriority.High,
            new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc));

        var newerMatch = SupportRequest.Create(
            "VPN instavel",
            "Quedas durante reunioes externas.",
            "Bruno Costa",
            RequestPriority.High,
            new DateTime(2026, 9, 12, 12, 0, 0, DateTimeKind.Utc));

        var differentPriority = SupportRequest.Create(
            "VPN lenta",
            "Lentidao no acesso remoto.",
            "Carla Souza",
            RequestPriority.Low,
            new DateTime(2026, 9, 12, 13, 0, 0, DateTimeKind.Utc));

        var differentSearch = SupportRequest.Create(
            "Monitor adicional",
            "Solicitacao de segundo monitor.",
            "Diego Lima",
            RequestPriority.High,
            new DateTime(2026, 9, 12, 14, 0, 0, DateTimeKind.Utc));

        await factory.SeedAsync(olderMatch, newerMatch, differentPriority, differentSearch);

        var response = await client.GetAsync(
            "/api/requests?status=Open&priority=High&search=VPN&pageNumber=1&pageSize=1");

        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<PagedResult<SupportRequestDto>>(JsonOptions);

        Assert.NotNull(page);
        Assert.Equal(2, page!.TotalItems);
        Assert.Equal(2, page.TotalPages);
        Assert.Single(page.Items);
        Assert.Equal("VPN instavel", page.Items.First().Title);
    }

    [Fact]
    public async Task GetRequest_WhenRecordDoesNotExist_ShouldReturnNotFound()
    {
        await factory.ResetDatabaseAsync();

        var response = await client.GetAsync($"/api/requests/{Guid.NewGuid()}");
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Solicitacao nao encontrada.", responseBody);
    }

    [Fact]
    public async Task ListRequests_WhenStatusValueIsInvalid_ShouldReturnBadRequest()
    {
        await factory.ResetDatabaseAsync();

        var response = await client.GetAsync("/api/requests?status=Invalid&pageNumber=1&pageSize=5");
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Falha de validacao", responseBody);
        Assert.Contains("A requisicao possui valores invalidos.", responseBody);
    }

    [Fact]
    public async Task UpdateRequest_WhenCompletedRequestReturnsToOpen_ShouldReturnConflict()
    {
        await factory.ResetDatabaseAsync();

        var created = await CreateRequestAsync();

        var completeResponse = await client.PatchAsJsonAsync(
            $"/api/requests/{created.Id}",
            new UpdateSupportRequestRequest
            {
                Priority = RequestPriority.High,
                Status = RequestStatus.Completed
            },
            JsonOptions);

        completeResponse.EnsureSuccessStatusCode();

        var reopenResponse = await client.PatchAsJsonAsync(
            $"/api/requests/{created.Id}",
            new UpdateSupportRequestRequest
            {
                Priority = RequestPriority.High,
                Status = RequestStatus.Open
            },
            JsonOptions);
        var responseBody = await reopenResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Conflict, reopenResponse.StatusCode);
        Assert.Contains("Uma solicitacao concluida nao pode ter o status alterado.", responseBody);
    }

    [Fact]
    public async Task DeleteRequest_WhenRequestIsNotOpen_ShouldReturnConflict()
    {
        await factory.ResetDatabaseAsync();

        var created = await CreateRequestAsync();

        var startResponse = await client.PatchAsJsonAsync(
            $"/api/requests/{created.Id}",
            new UpdateSupportRequestRequest
            {
                Priority = RequestPriority.Medium,
                Status = RequestStatus.InProgress
            },
            JsonOptions);

        startResponse.EnsureSuccessStatusCode();

        var deleteResponse = await client.DeleteAsync($"/api/requests/{created.Id}");
        var responseBody = await deleteResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);
        Assert.Contains("Apenas solicitacoes abertas podem ser excluidas.", responseBody);
    }

    [Fact]
    public async Task Health_WhenDatabaseIsAvailable_ShouldReturnHealthy()
    {
        await factory.ResetDatabaseAsync();

        var response = await client.GetAsync("/health");
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", responseBody);
    }

    private async Task<SupportRequestDto> CreateRequestAsync()
    {
        var response = await client.PostAsJsonAsync(
            "/api/requests",
            new CreateSupportRequestRequest
            {
                Title = "VPN corporativa",
                Description = "Usuario nao consegue acessar a VPN.",
                Requester = "Ana Silva",
                Priority = RequestPriority.Medium
            },
            JsonOptions);

        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<SupportRequestDto>(JsonOptions);
        Assert.NotNull(created);

        return created!;
    }
}

public sealed class SupportRequestsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string DefaultConnectionVariable = "ConnectionStrings__DefaultConnection";

    private readonly MsSqlContainer sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private string connectionString = string.Empty;
    private string? previousConnectionString;

    public async Task InitializeAsync()
    {
        await sqlServer.StartAsync();

        var builder = new SqlConnectionStringBuilder(sqlServer.GetConnectionString())
        {
            InitialCatalog = "RequestFlowTests"
        };

        connectionString = builder.ConnectionString;
        previousConnectionString = Environment.GetEnvironmentVariable(DefaultConnectionVariable);
        Environment.SetEnvironmentVariable(DefaultConnectionVariable, connectionString);
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SupportRequestsDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    public async Task SeedAsync(params SupportRequest[] supportRequests)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SupportRequestsDbContext>();

        await dbContext.SupportRequests.AddRangeAsync(supportRequests);
        await dbContext.SaveChangesAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Environment.SetEnvironmentVariable(DefaultConnectionVariable, previousConnectionString);
        await sqlServer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<SupportRequestsDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<SupportRequestsDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        });
    }
}
