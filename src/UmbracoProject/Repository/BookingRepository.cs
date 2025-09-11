// /Repository/BookingRepository.cs
using UmbracoProject.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace UmbracoProject.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IScopeProvider _scopeProvider;

        public BookingRepository(IScopeProvider scopeProvider) => _scopeProvider = scopeProvider;

        public async Task<int> GetBookedSeatsAsync(Guid tripId)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql(@"
                SELECT COUNT(1)
                FROM [Passenger] p
                INNER JOIN [Booking] b ON p.[BookingId] = b.[bookingId]
                WHERE b.[tripId] = @0", tripId);

            var booked = await db.ExecuteScalarAsync<int>(sql);
            scope.Complete();
            return booked;
        }

        public async Task CreateBookingAsync(Booking booking, IEnumerable<Passenger> passengers)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            await db.InsertAsync(booking);

            foreach (var p in passengers)
            {
                await db.InsertAsync(p);
            }

            scope.Complete();
        }

        public async Task<Booking?> GetBookingAsync(Guid bookingId)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Booking] WHERE [bookingId] = @0", bookingId);
            var row = await db.SingleOrDefaultAsync<Booking>(sql);
            scope.Complete();
            return row;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;
            var sql = new NPoco.Sql("SELECT * FROM [Booking] ORDER BY [date] DESC");
            var rows = await db.FetchAsync<Booking>(sql);
            scope.Complete();
            return rows;
        }

        public async Task<List<Passenger>> GetPassengersForBookingAsync(Guid bookingId)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Passenger] WHERE [BookingId] = @0", bookingId);
            var rows = await db.FetchAsync<Passenger>(sql);
            scope.Complete();
            return rows;
        }

        public async Task<List<Booking>> GetBookingsForTripAsync(Guid tripId)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [Booking] WHERE [tripId] = @0 ORDER BY [date] DESC", tripId);
            var rows = await db.FetchAsync<Booking>(sql);
            scope.Complete();
            return rows;
        }

        public async Task<bool> TryReserveSeatsAsync(Guid id, int seatCount)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;

            
            var rows = await db.ExecuteAsync(new NPoco.Sql(@"
            UPDATE [Trip]
            SET [passengerCount] = [passengerCount] - @0,
            [tripStatus] = CASE 
            WHEN [passengerCount] - @0 = 0 THEN @3 
            ELSE [tripStatus] 
            END
            WHERE [tripId] = @1
            AND [passengerCount] >= @0
            AND [tripStatus] = @2;", seatCount, id, (int)TripStatus.Schedueled, (int)TripStatus.SoldOut));

            return rows > 0;
        }


    }
}
