using MediatR;

namespace RequestFlow.Application.SupportRequests.Commands.DeleteSupportRequest;

public sealed record DeleteSupportRequestCommand(Guid Id) : IRequest;
