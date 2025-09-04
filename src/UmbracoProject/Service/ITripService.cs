using UmbracoProject.Models;
using UmbracoProject.DTO;

namespace UmbracoProject.Service
{
    public interface ITripService
    {
        Task<Guid> CreateTripAsync(CreateTripRequest request);
        Task<GetTripResponse> GetTripAsync(Guid id);
        Task <bool> UpdateTripStatusAsync(Guid id, TripStatus status);

        Task <List<GetTripResponse>> GetAllTripsAsync();

        Task<List<GetTripResponse>> GetAllTripsPriceAscAsync();

        Task<List<GetTripResponse>> GetAllTripsTravelTimeAscAsync();

        Task<List<GetTripResponse>> GetAllTripsByDestinationAsync(Guid destinationId);

        Task<List<GetTripResponse>> GetAvailableTripsAsync(int groupSize);

        Task<TripFilterResponse> GetFilteredTripsAsync(TripFilterRequest filter);

    }
}
