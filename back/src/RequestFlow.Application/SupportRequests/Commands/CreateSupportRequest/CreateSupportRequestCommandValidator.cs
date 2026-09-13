using FluentValidation;

namespace RequestFlow.Application.SupportRequests.Commands.CreateSupportRequest;

public sealed class CreateSupportRequestCommandValidator : AbstractValidator<CreateSupportRequestCommand>
{
    public CreateSupportRequestCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("O titulo e obrigatorio.")
            .MaximumLength(120).WithMessage("O titulo deve ter no maximo 120 caracteres.");

        RuleFor(command => command.Description)
            .NotEmpty().WithMessage("A descricao e obrigatoria.")
            .MaximumLength(2000).WithMessage("A descricao deve ter no maximo 2000 caracteres.");

        RuleFor(command => command.Requester)
            .NotEmpty().WithMessage("O solicitante e obrigatorio.")
            .MaximumLength(120).WithMessage("O solicitante deve ter no maximo 120 caracteres.");

        RuleFor(command => command.Priority)
            .IsInEnum().WithMessage("Prioridade invalida.");
    }
}
