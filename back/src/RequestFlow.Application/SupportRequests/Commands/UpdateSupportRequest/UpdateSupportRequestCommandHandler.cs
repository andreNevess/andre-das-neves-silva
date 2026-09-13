using RequestFlow.Application.Common.Exceptions;
using RequestFlow.Application.Common.Interfaces;
using RequestFlow.Application.SupportRequests.Dtos;
using RequestFlow.Domain.Common;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Commands.UpdateSupportRequest;

public sealed class UpdateSupportRequestCommandHandler : IRequestHandler<UpdateSupportRequestCommand, SupportRequestDto>
{
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly ISupportRequestRepository repository;

    public UpdateSupportRequestCommandHandler(
        IDateTimeProvider dateTimeProvider,
        ISupportRequestRepository repository)
    {
        this.dateTimeProvider = dateTimeProvider;
        this.repository = repository;
    }

    public async Task<SupportRequestDto> Handle(
        UpdateSupportRequestCommand request,
        CancellationToken cancellationToken)
    {
        var supportRequest = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (supportRequest is null)
        {
            throw new NotFoundException("Solicitacao nao encontrada.");
        }

        try
        {
            supportRequest.UpdatePriority(request.Priority);
            supportRequest.UpdateStatus(request.Status, dateTimeProvider.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new BusinessRuleException(exception.Message);
        }

        await repository.SaveChangesAsync(cancellationToken);

        return SupportRequestDto.FromEntity(supportRequest);
    }
}
