using FluentValidation;

namespace RequestFlow.Application.SupportRequests.Queries.ListSupportRequests;

public sealed class ListSupportRequestsQueryValidator : AbstractValidator<ListSupportRequestsQuery>
{
    public ListSupportRequestsQueryValidator()
    {
        RuleFor(query => query.Status)
            .IsInEnum()
            .When(query => query.Status.HasValue)
            .WithMessage("Status invalido.");

        RuleFor(query => query.Priority)
            .IsInEnum()
            .When(query => query.Priority.HasValue)
            .WithMessage("Prioridade invalida.");

        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("A pagina deve ser maior ou igual a 1.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da pagina deve estar entre 1 e 100.");

        RuleFor(query => query.Search)
            .MaximumLength(120).WithMessage("A pesquisa deve ter no maximo 120 caracteres.");
    }
}
