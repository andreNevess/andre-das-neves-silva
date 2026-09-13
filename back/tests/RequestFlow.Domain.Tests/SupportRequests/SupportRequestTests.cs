using RequestFlow.Domain.Common;
using RequestFlow.Domain.SupportRequests;

namespace RequestFlow.Domain.Tests.SupportRequests;

public sealed class SupportRequestTests
{
    [Fact]
    public void UpdateStatus_WhenStatusIsCompleted_ShouldSetCompletionDate()
    {
        var createdAtUtc = new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc);
        var completedAtUtc = createdAtUtc.AddHours(2);
        var supportRequest = SupportRequest.Create(
            "Notebook sem acesso",
            "Usuario nao consegue acessar a VPN.",
            "Ana Silva",
            RequestPriority.High,
            createdAtUtc);

        supportRequest.UpdateStatus(RequestStatus.Completed, completedAtUtc);

        Assert.Equal(RequestStatus.Completed, supportRequest.Status);
        Assert.Equal(completedAtUtc, supportRequest.CompletedAtUtc);
    }

    [Fact]
    public void UpdateStatus_WhenCompletedRequestReturnsToOpen_ShouldThrowDomainException()
    {
        var createdAtUtc = new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc);
        var supportRequest = SupportRequest.Create(
            "Reset de acesso",
            "Solicitacao de reset de senha.",
            "Bruno Costa",
            RequestPriority.Medium,
            createdAtUtc);

        supportRequest.UpdateStatus(RequestStatus.Completed, createdAtUtc.AddHours(1));

        var exception = Assert.Throws<DomainException>(() =>
            supportRequest.UpdateStatus(RequestStatus.Open, createdAtUtc.AddHours(2)));

        Assert.Equal("Uma solicitacao concluida nao pode ter o status alterado.", exception.Message);
    }

    [Fact]
    public void UpdateStatus_WhenCompletedRequestReturnsToInProgress_ShouldThrowDomainException()
    {
        var createdAtUtc = new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc);
        var supportRequest = SupportRequest.Create(
            "Instalacao de software",
            "Instalar ferramenta aprovada pelo time de seguranca.",
            "Fernanda Lima",
            RequestPriority.Medium,
            createdAtUtc);

        supportRequest.UpdateStatus(RequestStatus.Completed, createdAtUtc.AddHours(1));

        var exception = Assert.Throws<DomainException>(() =>
            supportRequest.UpdateStatus(RequestStatus.InProgress, createdAtUtc.AddHours(2)));

        Assert.Equal("Uma solicitacao concluida nao pode ter o status alterado.", exception.Message);
    }

    [Fact]
    public void EnsureCanBeDeleted_WhenRequestIsNotOpen_ShouldThrowDomainException()
    {
        var createdAtUtc = new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc);
        var supportRequest = SupportRequest.Create(
            "Acesso ao CRM",
            "Liberar perfil de leitura.",
            "Carla Souza",
            RequestPriority.Low,
            createdAtUtc);

        supportRequest.UpdateStatus(RequestStatus.InProgress, createdAtUtc.AddMinutes(30));

        var exception = Assert.Throws<DomainException>(supportRequest.EnsureCanBeDeleted);

        Assert.Equal("Apenas solicitacoes abertas podem ser excluidas.", exception.Message);
    }
}
