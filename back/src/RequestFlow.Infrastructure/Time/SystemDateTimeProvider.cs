using RequestFlow.Application.Common.Interfaces;

namespace RequestFlow.Infrastructure.Time;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
