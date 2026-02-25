public class PaginationViewModel
{
    public int CurrentPageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasNextPage => CurrentPageNumber < TotalPages;
    public bool HasPreviousPage => CurrentPageNumber > 1;
}   