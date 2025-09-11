using UmbracoProject.DTO;

namespace UmbracoProject.Service
{
    public interface IBookingService
    {
        Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request);
        
        Task<bool> CancelBookingAsync(Guid bookingId);
        Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId);

        Task<List<GetBookingResponse>> GetAllBookingsAsync();

        Task<List<GetBookingResponse>> GetByTripAsync(Guid tripId);

        Task<bool> TryReserveSeatsAsync(Guid id, int seats);

    }
}
