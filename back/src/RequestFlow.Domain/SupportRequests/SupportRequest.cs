using RequestFlow.Domain.Common;

namespace RequestFlow.Domain.SupportRequests;

public sealed class SupportRequest
{
    private SupportRequest()
    {
    }

    private SupportRequest(
        Guid id,
        string title,
        string description,
        string requester,
        RequestPriority priority,
        DateTime createdAtUtc)
    {
        Id = id;
        Title = title;
        Description = description;
        Requester = requester;
        Priority = priority;
        Status = RequestStatus.Open;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string Requester { get; private set; } = string.Empty;

    public RequestPriority Priority { get; private set; }

    public RequestStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public static SupportRequest Create(
        string title,
        string description,
        string requester,
        RequestPriority priority,
        DateTime createdAtUtc)
    {
        EnsureRequired(title, "O titulo e obrigatorio.");
        EnsureRequired(description, "A descricao e obrigatoria.");
        EnsureRequired(requester, "O solicitante e obrigatorio.");

        if (!Enum.IsDefined(priority))
        {
            throw new DomainException("Prioridade invalida.");
        }

        return new SupportRequest(
            Guid.NewGuid(),
            title.Trim(),
            description.Trim(),
            requester.Trim(),
            priority,
            DateTime.SpecifyKind(createdAtUtc, DateTimeKind.Utc));
    }

    public void UpdatePriority(RequestPriority priority)
    {
        if (!Enum.IsDefined(priority))
        {
            throw new DomainException("Prioridade invalida.");
        }

        Priority = priority;
    }

    public void UpdateStatus(RequestStatus status, DateTime changedAtUtc)
    {
        if (!Enum.IsDefined(status))
        {
            throw new DomainException("Status invalido.");
        }

        if (Status == RequestStatus.Completed && status != RequestStatus.Completed)
        {
            throw new DomainException("Uma solicitacao concluida nao pode ter o status alterado.");
        }

        Status = status;
        CompletedAtUtc = status == RequestStatus.Completed
            ? DateTime.SpecifyKind(changedAtUtc, DateTimeKind.Utc)
            : null;
    }

    public void EnsureCanBeDeleted()
    {
        if (Status != RequestStatus.Open)
        {
            throw new DomainException("Apenas solicitacoes abertas podem ser excluidas.");
        }
    }

    private static void EnsureRequired(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }
    }
}
