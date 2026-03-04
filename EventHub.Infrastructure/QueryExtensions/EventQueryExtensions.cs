namespace EventHub.Infrastructure.QueryExtensions
{
    using EventHub.Core.Models.Events;

    public static class EventQueryExtensions
    {
        public static IQueryable<Event> FilterByTitle(
            this IQueryable<Event> query,
            string? title)
        {
            if (string.IsNullOrEmpty(title))
                return query;

            return query.Where(x =>x.Title.ToLower().Contains(title.Trim().ToLower()));
        }

        public static IQueryable<Event> FilterByDate(
            this IQueryable<Event> query,
            DateTime? startDate,
            DateTime? endDate)
        {
            if (!startDate.HasValue && !endDate.HasValue)
                return query;

            if (startDate.HasValue)
                query = query.Where(x => x.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.EndDate <= endDate.Value);

            return query;
        }

        public static IQueryable<Event> FilterByCategory(
           this IQueryable<Event> query,
           Guid?  categoryId)
        {
            if (!categoryId.HasValue || categoryId == Guid.Empty)
                return query;

            return query.Where(x=>x.CategoryId == categoryId);
        }

        public static IQueryable<Event> FilterByLocation(
          this IQueryable<Event> query,
          Guid? locationId)
        {
            if (!locationId.HasValue || locationId == Guid.Empty)
                return query;

            return query.Where(x => x.LocationId == locationId);
        }
    }
}
