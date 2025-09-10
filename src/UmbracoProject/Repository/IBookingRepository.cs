using UmbracoProject.Models;

namespace UmbracoProject.Repository
{
    public interface IBookingRepository
    {
        Task<int> GetBookedSeatsAsync(Guid tripId);

        Task CreateBookingAsync(Booking booking, IEnumerable<Passenger> passengers);

        Task<Booking?> GetBookingAsync(Guid bookingId);

        Task<List<Booking>> GetAllBookingsAsync();

        Task<List<Passenger>> GetPassengersForBookingAsync(Guid bookingId);

        Task<List<Booking>> GetBookingsForTripAsync(Guid tripId);

        Task<bool> TryReserveSeatsAsync(Guid id, int seatCount);
    }
}
