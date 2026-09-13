using RequestFlow.Application.SupportRequests.Dtos;
using RequestFlow.Domain.SupportRequests;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Commands.UpdateSupportRequest;

public sealed record UpdateSupportRequestCommand(
    Guid Id,
    RequestPriority Priority,
    RequestStatus Status) : IRequest<SupportRequestDto>;
