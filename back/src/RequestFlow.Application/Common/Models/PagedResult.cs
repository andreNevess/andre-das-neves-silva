namespace RequestFlow.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int TotalItems,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => TotalItems == 0
        ? 0
        : (int)Math.Ceiling(TotalItems / (double)PageSize);
}
