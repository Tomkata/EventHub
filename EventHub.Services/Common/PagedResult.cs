

namespace EventHub.Services.Common
{
    public class PagedResult<T>
    {
        public PagedResult()
        {
            
        }
        public List<T> Data { get; init; }
        public int CurrentPageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages =>
        (int) (Math.Ceiling((double) TotalRecords / PageSize));

        public int TotalRecords { get; init; }
        public bool HasNextPage => this.CurrentPageNumber < this.TotalPages;
        public bool HasPreviousPage => this.CurrentPageNumber > 1;

    }
}
