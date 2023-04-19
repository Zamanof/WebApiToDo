namespace TO_DO.DTOs.Pagination;

public class PaginationMeta
{

    public PaginationMeta(int page, int pageSize, int count)
    {
        Page = page;
        PageSize = pageSize;
        TotalPages = (count + pageSize - 1) / pageSize;
    }

    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages { get; }

}