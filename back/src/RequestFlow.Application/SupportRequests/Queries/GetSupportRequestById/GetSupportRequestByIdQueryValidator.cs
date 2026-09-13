using FluentValidation;

namespace RequestFlow.Application.SupportRequests.Queries.GetSupportRequestById;

public sealed class GetSupportRequestByIdQueryValidator : AbstractValidator<GetSupportRequestByIdQuery>
{
    public GetSupportRequestByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage("O identificador da solicitacao e obrigatorio.");
    }
}
