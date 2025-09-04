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

        public async Task<List<Trip>> GetAllTripsPriceAscAsync()
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Trip] ORDER BY price ASC");
            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
        }

        public async Task<List<Trip>> GetAllTripsTravelTimeAscAsync()
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT  t.*\r\nFROM    dbo.[Trip] AS t WHERE   t.departureUtc IS NOT NULL AND t.arrivalUtc IS NOT NULL AND t.arrivalUtc > t.departureUtc ORDER BY DATEDIFF(SECOND, t.departureUtc, t.arrivalUtc) ASC;");
            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
        }

        public async Task<List<Trip>> GetAllTripsByDestination(Guid destinationId)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Trip] WHERE [destinationKey] = @0", destinationId);
            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
        }

        public async Task<List<Trip>> GetScheduledTripsWithMinCapacityAsync(int groupSize)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql(
                @"SELECT * FROM [Trip]
          WHERE [tripStatus] = @1
            AND [passengerCount] >= @0
          ORDER BY [departureUtc] ASC",
                groupSize, (int)TripStatus.Schedueled);

            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
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
                // Compare DATE to DATE (time ignored)
                var d = filter.DepartureDate.Value.ToDateTime(TimeOnly.MinValue);
                sql.Append(" AND CAST([departureUtc] AS DATE) = @0 ", d);
            }

            if (filter.PassengerCount.HasValue)
                sql.Append(" AND [passengerCount] >= @0 ", filter.PassengerCount.Value);

            sql.Append(" ORDER BY [departureUtc] ASC ");

            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
        }


        public async Task<List<Trip>> FindNearbyTripsAsync(TripFilterRequest filter, bool excludeExact = false)
        {
            if (!filter.DepartureDate.HasValue) return new List<Trip>();

            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var search = filter.DepartureDate.Value;
            var start = search.AddDays(-filter.NearbyDaysRange).ToDateTime(TimeOnly.MinValue);
            var end = search.AddDays(filter.NearbyDaysRange).ToDateTime(TimeOnly.MinValue);

            var sql = new NPoco.Sql("SELECT * FROM [Trip] WHERE [tripStatus] = @0 ", (int)TripStatus.Schedueled);

            // Date-only window (inclusive): CAST(... AS DATE) BETWEEN start AND end
            sql.Append(" AND CAST([departureUtc] AS DATE) BETWEEN @0 AND @1 ", start, end);

            if (excludeExact)
            {
                var exact = search.ToDateTime(TimeOnly.MinValue);
                sql.Append(" AND CAST([departureUtc] AS DATE) <> @0 ", exact);
            }

            if (filter.DestinationKey.HasValue)
                sql.Append(" AND [destinationKey] = @0 ", filter.DestinationKey.Value);

            if (filter.PassengerCount.HasValue)
                sql.Append(" AND [passengerCount] >= @0 ", filter.PassengerCount.Value);

            // Sort by closeness to the searched DATE
            sql.Append(" ORDER BY ABS(DATEDIFF(day, CAST([departureUtc] AS DATE), @0)), [departureUtc] ",
                       search.ToDateTime(TimeOnly.MinValue));

            var trips = await db.FetchAsync<Trip>(sql);
            scope.Complete();
            return trips;
        }

    }
}
