using UmbracoProject.DTO;

namespace UmbracoProject.Service
{
    public interface IBookingService
    {
        Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request);

        Task<GetBookingResponse> GetAsync(Guid bookingId);

        Task<List<GetBookingResponse>> GetByTripAsync(Guid tripId);

        Task<bool> TryReserveSeatsAsync(Guid id, int seats);
    }
}
