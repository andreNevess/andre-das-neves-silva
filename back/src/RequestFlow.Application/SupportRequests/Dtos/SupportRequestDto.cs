using RequestFlow.Domain.SupportRequests;

namespace RequestFlow.Application.SupportRequests.Dtos;

public sealed record SupportRequestDto(
    Guid Id,
    string Title,
    string Description,
    string Requester,
    RequestPriority Priority,
    RequestStatus Status,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc)
{
    public static SupportRequestDto FromEntity(SupportRequest supportRequest)
    {
        return new SupportRequestDto(
            supportRequest.Id,
            supportRequest.Title,
            supportRequest.Description,
            supportRequest.Requester,
            supportRequest.Priority,
            supportRequest.Status,
            supportRequest.CreatedAtUtc,
            supportRequest.CompletedAtUtc);
    }
}
