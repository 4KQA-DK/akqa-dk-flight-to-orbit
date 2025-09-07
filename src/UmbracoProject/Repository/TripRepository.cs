using UmbracoProject.Models;
using Umbraco.Cms.Infrastructure.Scoping;
using UmbracoProject.DTO;

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

    

        public async Task<List<Trip>> GetFilteredTripsAsync(TripFilterRequest filter)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Trip] WHERE [tripStatus] = @0 ", (int)TripStatus.Schedueled);

            if (filter.DestinationKey.HasValue)
                sql.Append(" AND [destinationKey] = @0 ", filter.DestinationKey.Value);

            if (filter.DepartureDate.HasValue)
            {
                var d = filter.DepartureDate.Value.ToDateTime(TimeOnly.MinValue);
                sql.Append(" AND CAST([departureUtc] AS DATE) = @0 ", d);
            }

            if (filter.PassengerCount.HasValue)
                sql.Append(" AND [passengerCount] >= @0 ", filter.PassengerCount.Value);

            sql.Append(BuildOrderBy(filter.SortBy));

            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
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

            var result = prev.Concat(next)
            .OrderBy(t => Math.Abs((t.departureUtc.Date - day.Date).TotalDays))
            .ThenBy(t => t.departureUtc)
            .ToList();

            return result;
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
    }

}
