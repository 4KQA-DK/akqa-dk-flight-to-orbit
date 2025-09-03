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

        // <summary>
        /// Filters scheduled trips based on criteria and enriches data with rocket and destination information.
        /// </summary>
        /// <param name="filter">Trip filtering criteria</param>
        /// <returns>List of enriched trip responses with rocket names, destination names, and trip details</returns>
        /// <exception cref="ArgumentException">Thrown when no scheduled trips match the filter criteria</exception>
        /// <exception cref="InvalidOperationException">Thrown when database error occurs during filtering</exception>
        Task<List<GetTripResponse>> FilterTripsAsync(TripFilterRequest filter);

    }
}
