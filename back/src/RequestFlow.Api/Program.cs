using System.Reflection;
using System.Text.Json.Serialization;
using RequestFlow.Api.HealthChecks;
using RequestFlow.Api.Middleware;
using RequestFlow.Application;
using RequestFlow.Infrastructure;
using RequestFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? BuildDefaultDockerConnectionString(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("sqlserver");

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Falha de validacao",
            Detail = "A requisicao possui valores invalidos.",
            Instance = context.HttpContext.Request.Path
        };

        AddCorrelationId(context.HttpContext, problem);

        return new BadRequestObjectResult(problem);
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:3000"];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Solicitacoes Internas",
        Version = "v1",
        Description = "API para registrar, acompanhar e concluir solicitacoes internas de suporte."
    });

    options.UseInlineDefinitionsForEnums();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEnd");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static string BuildDefaultDockerConnectionString(IConfiguration configuration)
{
    var password = configuration["MSSQL_SA_PASSWORD"];

    if (string.IsNullOrWhiteSpace(password))
    {
        throw new InvalidOperationException(
            "MSSQL_SA_PASSWORD was not configured. Set it before running the API or provide ConnectionStrings__DefaultConnection.");
    }

    return SqlServerConnectionStringFactory.Create(
        configuration["SqlServer:Server"] ?? "localhost,1433",
        configuration["SqlServer:Database"] ?? "RequestFlowRequestsDb",
        configuration["SqlServer:User"] ?? "sa",
        password);
}

static void AddCorrelationId(HttpContext context, ProblemDetails problem)
{
    if (context.Items.TryGetValue(CorrelationIdMiddleware.ItemName, out var value)
        && value is string correlationId)
    {
        problem.Extensions["correlationId"] = correlationId;
    }
}

public partial class Program
{
}
