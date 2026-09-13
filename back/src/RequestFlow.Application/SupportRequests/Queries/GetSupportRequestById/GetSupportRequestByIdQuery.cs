using RequestFlow.Application.SupportRequests.Dtos;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Queries.GetSupportRequestById;

public sealed record GetSupportRequestByIdQuery(Guid Id) : IRequest<SupportRequestDto>;
