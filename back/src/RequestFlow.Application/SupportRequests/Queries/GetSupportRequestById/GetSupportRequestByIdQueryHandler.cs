using RequestFlow.Application.Common.Exceptions;
using RequestFlow.Application.Common.Interfaces;
using RequestFlow.Application.SupportRequests.Dtos;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Queries.GetSupportRequestById;

public sealed class GetSupportRequestByIdQueryHandler : IRequestHandler<GetSupportRequestByIdQuery, SupportRequestDto>
{
    private readonly ISupportRequestRepository repository;

    public GetSupportRequestByIdQueryHandler(ISupportRequestRepository repository)
    {
        this.repository = repository;
    }

    public async Task<SupportRequestDto> Handle(
        GetSupportRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supportRequest = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (supportRequest is null)
        {
            throw new NotFoundException("Solicitacao nao encontrada.");
        }

        return SupportRequestDto.FromEntity(supportRequest);
    }
}
