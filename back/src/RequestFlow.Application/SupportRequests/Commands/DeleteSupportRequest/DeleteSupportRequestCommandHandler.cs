using RequestFlow.Application.Common.Exceptions;
using RequestFlow.Application.Common.Interfaces;
using RequestFlow.Domain.Common;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Commands.DeleteSupportRequest;

public sealed class DeleteSupportRequestCommandHandler : IRequestHandler<DeleteSupportRequestCommand>
{
    private readonly ISupportRequestRepository repository;

    public DeleteSupportRequestCommandHandler(ISupportRequestRepository repository)
    {
        this.repository = repository;
    }

    public async Task Handle(DeleteSupportRequestCommand request, CancellationToken cancellationToken)
    {
        var supportRequest = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (supportRequest is null)
        {
            throw new NotFoundException("Solicitacao nao encontrada.");
        }

        try
        {
            supportRequest.EnsureCanBeDeleted();
        }
        catch (DomainException exception)
        {
            throw new BusinessRuleException(exception.Message);
        }

        repository.Remove(supportRequest);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
