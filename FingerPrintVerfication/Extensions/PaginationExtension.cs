namespace FingerPrintVerfication.Extensions;

static public class PaginationExtension
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}