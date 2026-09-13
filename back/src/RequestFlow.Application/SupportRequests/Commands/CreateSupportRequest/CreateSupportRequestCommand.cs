using RequestFlow.Application.SupportRequests.Dtos;
using RequestFlow.Domain.SupportRequests;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Commands.CreateSupportRequest;

public sealed record CreateSupportRequestCommand(
    string Title,
    string Description,
    string Requester,
    RequestPriority Priority) : IRequest<SupportRequestDto>;
