using Microsoft.EntityFrameworkCore;

namespace Takeel.Application.Contracts;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalRows { get; set; }

    public PaginatedResponse(IEnumerable<T> items, int currentPage, int totalPages, int totalRows)
    {
        Items = items;
        CurrentPage = currentPage;
        TotalPages = totalPages;
        TotalRows = totalRows;
    }

    public static async Task<PaginatedResponse<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalRows = await query.CountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return new PaginatedResponse<T>(items, pageNumber, totalPages, totalRows);
    }

    public static PaginatedResponse<T> Paginate(List<T> source, int pageNumber, int pageSize)
    {
        var totalRows = source.Count;
        var totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResponse<T>(items, pageNumber, totalPages, totalRows);
    }
}