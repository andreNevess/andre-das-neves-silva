using RequestFlow.Application.Common.Interfaces;
using RequestFlow.Application.SupportRequests.Dtos;
using RequestFlow.Domain.SupportRequests;
using MediatR;

namespace RequestFlow.Application.SupportRequests.Commands.CreateSupportRequest;

public sealed class CreateSupportRequestCommandHandler : IRequestHandler<CreateSupportRequestCommand, SupportRequestDto>
{
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly ISupportRequestRepository repository;

    public CreateSupportRequestCommandHandler(
        IDateTimeProvider dateTimeProvider,
        ISupportRequestRepository repository)
    {
        this.dateTimeProvider = dateTimeProvider;
        this.repository = repository;
    }

    public async Task<SupportRequestDto> Handle(
        CreateSupportRequestCommand request,
        CancellationToken cancellationToken)
    {
        var supportRequest = SupportRequest.Create(
            request.Title,
            request.Description,
            request.Requester,
            request.Priority,
            dateTimeProvider.UtcNow);

        await repository.AddAsync(supportRequest, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return SupportRequestDto.FromEntity(supportRequest);
    }
}
