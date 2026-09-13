using FluentValidation;

namespace RequestFlow.Application.SupportRequests.Commands.DeleteSupportRequest;

public sealed class DeleteSupportRequestCommandValidator : AbstractValidator<DeleteSupportRequestCommand>
{
    public DeleteSupportRequestCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("O identificador da solicitacao e obrigatorio.");
    }
}
