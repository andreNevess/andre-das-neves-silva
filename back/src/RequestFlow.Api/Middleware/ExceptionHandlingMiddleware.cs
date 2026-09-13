using RequestFlow.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace RequestFlow.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (RequestValidationException exception)
        {
            await WriteValidationProblemAsync(context, exception);
        }
        catch (NotFoundException exception)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status404NotFound,
                "Registro nao encontrado",
                exception.Message);
        }
        catch (BusinessRuleException exception)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status409Conflict,
                "Regra de negocio violada",
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled API exception.");

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Erro inesperado",
                "Ocorreu um erro inesperado ao processar a solicitacao.");
        }
    }

    private static async Task WriteValidationProblemAsync(
        HttpContext context,
        RequestValidationException exception)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var problem = new HttpValidationProblemDetails(
            exception.Errors.ToDictionary(error => error.Key, error => error.Value))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Falha de validacao",
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        AddCorrelationId(context, problem);

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        AddCorrelationId(context, problem);

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static void AddCorrelationId(HttpContext context, ProblemDetails problem)
    {
        if (context.Items.TryGetValue(CorrelationIdMiddleware.ItemName, out var value)
            && value is string correlationId)
        {
            problem.Extensions["correlationId"] = correlationId;
        }
    }
}
