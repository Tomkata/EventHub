

namespace EventHub.Services.Common
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Immutable;
    public static class PaginationExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,                                    
            int pageNumber,                                                   
            int pageSize,
            CancellationToken cancellationToken)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);



            var totalRecords = await query.CountAsync();

            var skip = (pageNumber - 1) * pageSize;
            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);


            return new PagedResult<T>
            {
                Data = data,
                CurrentPageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
        
    }
}
