namespace RequestFlow.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";
    public const string ItemName = "CorrelationId";

    private readonly RequestDelegate next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        context.Items[ItemName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope("{CorrelationId}", correlationId))
        {
            await next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var values)
            && !string.IsNullOrWhiteSpace(values.FirstOrDefault()))
        {
            return values.First()!;
        }

        return context.TraceIdentifier;
    }
}
