namespace TO_DO.DTOs.Pagination;

public class PaginationMeta
{

    public PaginationMeta(int page, int pageSize, int totalPages)
    {
        Page = page;
        PageSize = pageSize;
        TotalPages = Convert.ToInt32(Math.Ceiling(1.0 * totalPages / pageSize));
    }

    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages { get; }

}