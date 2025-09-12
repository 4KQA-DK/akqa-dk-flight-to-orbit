using UmbracoProject.Models;
using Umbraco.Cms.Infrastructure.Scoping;
using UmbracoProject.DTO;
using UmbracoProject.Pagination;

namespace UmbracoProject.Repository
{
    public class TripRepository : ITripRepository
    {
        private readonly IScopeProvider _scopeProvider;

        public TripRepository(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task CreateTripAsync(Trip trip)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;
            await db.InsertAsync(trip);
            scope.Complete();
        }

        public async Task<Trip?> GetTripByIdAsync(Guid tripId)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Trip] WHERE [tripId] = @0", tripId);
            var trip = await db.SingleOrDefaultAsync<Trip>(sql);
            scope.Complete();
            return trip;

        }

        public async Task<List<Trip>> GetAllTripsAsync()
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Trip]");
            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, TripStatus status)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;

            var tripsAffected = await db.ExecuteAsync( new NPoco.Sql("UPDATE [Trip] SET [tripStatus] = @0 WHERE [tripId] = @1",(int)status, id));

            return tripsAffected > 0;
        }

        public async Task UpdateAvaliableSeatsCountAsync(Guid id, int avaliableSeats)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;

            var rows = await db.ExecuteAsync(new NPoco.Sql(@"
                UPDATE [Trip]
                SET [passengerCount] = @0
                WHERE [tripId] = @0;",
                avaliableSeats, id));

            scope.Complete();
        }


        public async Task<PagedList<Trip>> GetFilteredTripsAsync(TripFilterRequest filter, PageParameters page)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            // Build the base WHERE clause
            var whereClause = new NPoco.Sql($"WHERE [tripStatus] = {(int)TripStatus.Schedueled}");

            if (filter.DestinationKey.HasValue)
                whereClause.Append($"WHERE [tripStatus] = {filter.DestinationKey.Value}");

            if (filter.DepartureDate.HasValue)
            {
                var d = filter.DepartureDate.Value.ToDateTime(TimeOnly.MinValue);
                whereClause.Append($"AND CAST([departureUtc] AS DATE) = {d}");
            }

            if (filter.PassengerCount.HasValue)
                whereClause.Append($"WHERE [tripStatus] = {filter.PassengerCount.Value}");

            var countSql = new NPoco.Sql("SELECT COUNT(*) FROM [Trip] ").Append(whereClause);
            var totalCount = await db.ExecuteScalarAsync<int>(countSql);

            var orderBy = BuildOrderBy(filter.SortBy);
            if (string.IsNullOrWhiteSpace(orderBy))
                orderBy = "ORDER BY [departureUtc] ASC";

            var pageNumber = Math.Max(1, page?.PageNumber ?? 1);
            var pageSize = Math.Max(1, page?.PageSize ?? 20);
            var offset = (pageNumber - 1) * pageSize;

            var sql = new NPoco.Sql("SELECT * FROM [Trip] ")
                .Append(whereClause)
                .Append(orderBy)
                .Append($"OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY");

            var items = await db.FetchAsync<Trip>(sql);

            scope.Complete();
            return new PagedList<Trip>(items, pageNumber, pageSize, totalCount);
        }

        private static string BuildOrderBy(TripSortBy by)
        {
            if (by == TripSortBy.Price)
            {
                return " ORDER BY [price] ASC, [departureUtc] ASC, [tripId] ASC";
            }

            if (by == TripSortBy.Duration)
            {
                return " ORDER BY DATEDIFF(SECOND, [departureUtc], [arrivalUtc]) ASC, [departureUtc] ASC, [tripId] ASC";
            }
                
            return " ORDER BY [departureUtc] ASC, [tripId] ASC";
        }


        public async Task<List<Trip>> FindNearbyTripsAsync(TripFilterRequest filter)
        { 
            if (!filter.DepartureDate.HasValue) return new();

            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var day = filter.DepartureDate.Value.ToDateTime(TimeOnly.MinValue);

            // 5 FØR
            var prevSql = new NPoco.Sql(
                @"SELECT TOP (5) * FROM [Trip]
                WHERE [tripStatus] = @0
                AND CAST([departureUtc] AS DATE) < @1", (int)TripStatus.Schedueled, day);

            if (filter.DestinationKey.HasValue)
                prevSql.Append(" AND [destinationKey] = @0", filter.DestinationKey.Value);

            if (filter.PassengerCount.HasValue)
                prevSql.Append(" AND [passengerCount] >= @0", filter.PassengerCount.Value);

            prevSql.Append(" ORDER BY [departureUtc] DESC"); 

            // 5 EFTER
            var nextSql = new NPoco.Sql(
                @"SELECT TOP (5) * FROM [Trip]
                WHERE [tripStatus] = @0
                AND CAST([departureUtc] AS DATE) > @1", (int)TripStatus.Schedueled, day);

            if (filter.DestinationKey.HasValue)
                nextSql.Append(" AND [destinationKey] = @0", filter.DestinationKey.Value);
            
            if (filter.PassengerCount.HasValue)
                nextSql.Append(" AND [passengerCount] >= @0", filter.PassengerCount.Value);

            nextSql.Append(" ORDER BY [departureUtc] ASC");

            var prev = await db.FetchAsync<Trip>(prevSql);
            var next = await db.FetchAsync<Trip>(nextSql);
            scope.Complete();

            var merged = prev.Concat(next);

            IOrderedEnumerable<Trip> ordered;

            if (filter.SortBy == TripSortBy.Price)
            {
                ordered = merged
                    .OrderBy(t => t.price)
                    .ThenBy(t => Math.Abs((t.departureUtc.Date - day.Date).TotalDays));
            }
            else if (filter.SortBy == TripSortBy.Duration)
            {
                ordered = merged
                    .OrderBy(t => (t.arrivalUtc - t.departureUtc).TotalSeconds)
                    .ThenBy(t => Math.Abs((t.departureUtc.Date - day.Date).TotalDays));
            }
            else 
            {
                ordered = merged
                    .OrderBy(t => Math.Abs((t.departureUtc.Date - day.Date).TotalDays));
            }

            return ordered.ToList();

        }

        public async Task<bool> HasOverlappingTripByDateAsync(Guid rocketKey, DateOnly startDate, DateOnly endDate,int turnaroundDays)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var fromDate = startDate.AddDays(-turnaroundDays).ToDateTime(TimeOnly.MinValue);
            var toDate = endDate.AddDays(turnaroundDays).ToDateTime(TimeOnly.MaxValue);

            var sql = new NPoco.Sql(@"
            SELECT TOP (1) 1
            FROM [Trip]
            WHERE [rocketKey] = @0
            AND [tripStatus] IN (@1, @2)
            AND [departureUtc] <= @3
            AND [arrivalUtc]   >= @4", rocketKey, (int)TripStatus.Schedueled, (int)TripStatus.Ongoing, toDate, fromDate);

            var row = await db.SingleOrDefaultAsync<int?>(sql);
            scope.Complete();
            return row.HasValue;
        }


        public async Task<int> UpdateTripStatusBackgroundServiceAsync()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;

           
            var toOngoing = await db.ExecuteAsync(@"
            UPDATE Trip
            SET tripStatus = 1
            WHERE tripStatus = 0
            AND departureUtc <= DATEADD(hour, 2, SYSUTCDATETIME())
            AND arrivalUtc  >  DATEADD(hour, 2, SYSUTCDATETIME());");

            var toCompleted = await db.ExecuteAsync(@"
            UPDATE Trip
            SET tripStatus = 2
            WHERE tripStatus = 1
            AND arrivalUtc  <= DATEADD(hour, 2, SYSUTCDATETIME());");

            return toOngoing + toCompleted;
        }

    }

}
