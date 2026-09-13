using FluentValidation;

namespace RequestFlow.Application.SupportRequests.Commands.UpdateSupportRequest;

public sealed class UpdateSupportRequestCommandValidator : AbstractValidator<UpdateSupportRequestCommand>
{
    public UpdateSupportRequestCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("O identificador da solicitacao e obrigatorio.");

        RuleFor(command => command.Priority)
            .IsInEnum().WithMessage("Prioridade invalida.");

        RuleFor(command => command.Status)
            .IsInEnum().WithMessage("Status invalido.");
    }
}
