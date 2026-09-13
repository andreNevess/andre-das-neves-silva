using RequestFlow.Domain.SupportRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RequestFlow.Infrastructure.Persistence.Configurations;

public sealed class SupportRequestConfiguration : IEntityTypeConfiguration<SupportRequest>
{
    public void Configure(EntityTypeBuilder<SupportRequest> builder)
    {
        builder.ToTable("SupportRequests", table =>
        {
            table.HasCheckConstraint(
                "CK_SupportRequests_Priority",
                "[Priority] IN ('Low', 'Medium', 'High')");

            table.HasCheckConstraint(
                "CK_SupportRequests_Status",
                "[Status] IN ('Open', 'InProgress', 'Completed')");

            table.HasCheckConstraint(
                "CK_SupportRequests_CompletedAtUtc",
                "([Status] = 'Completed' AND [CompletedAtUtc] IS NOT NULL) OR ([Status] <> 'Completed' AND [CompletedAtUtc] IS NULL)");
        });

        builder.HasKey(supportRequest => supportRequest.Id);

        builder.Property(supportRequest => supportRequest.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(supportRequest => supportRequest.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(supportRequest => supportRequest.Requester)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(supportRequest => supportRequest.Priority)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(supportRequest => supportRequest.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(supportRequest => supportRequest.CreatedAtUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(supportRequest => supportRequest.CompletedAtUtc)
            .HasColumnType("datetime2");

        builder.HasIndex(supportRequest => new
            {
                supportRequest.Status,
                supportRequest.Priority,
                supportRequest.CreatedAtUtc
            })
            .HasDatabaseName("IX_SupportRequests_Status_Priority_CreatedAtUtc");

        builder.HasIndex(supportRequest => supportRequest.Title)
            .HasDatabaseName("IX_SupportRequests_Title");

        builder.HasIndex(supportRequest => supportRequest.Requester)
            .HasDatabaseName("IX_SupportRequests_Requester");
    }
}
