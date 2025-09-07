using UmbracoProject.DTO;
using UmbracoProject.Models;

namespace UmbracoProject.Service
{
    public interface IAdminTripService
    {
        Task<Guid> CreateTripAsync(CreateTripRequest request);
        Task<List<GetTripResponse>> GetAllTripsAsync();
        Task<GetTripResponse> GetTripAsync(Guid id);
        Task<bool> UpdateTripStatusAsync(Guid id, TripStatus status);
        Task CancelTripAsync(Guid tripId);

    }
}
