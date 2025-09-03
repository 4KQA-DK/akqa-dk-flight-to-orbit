using UmbracoProject.Models;
using UmbracoProject.DTO;

namespace UmbracoProject.Repository
{
    public interface ITripRepository
    {
        Task CreateTripAsync(Trip trip);
        Task<Trip?> GetTripByIdAsync(Guid tripId);

        Task<List<Trip>> GetAllTripsAsync();

        Task<bool> UpdateStatusAsync(Guid id, TripStatus status);

        Task<List<Trip>> GetAllTripsPriceAscAsync();

        Task<List<Trip>> GetAllTripsTravelTimeAscAsync();

        Task<List<Trip>> GetAllTripsByDestination(Guid destinationId);

        Task<List<Trip>> GetScheduledTripsWithMinCapacityAsync(int groupSize);

        /// <summary>
        /// Queries database for scheduled trips matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">Filter parameters for trip search</param>
        /// <returns>List of raw trip entities from database matching the filter conditions</returns>
        /// <remarks>
        /// Builds dynamic SQL query filtering on:
        /// - Trip status (always scheduled)
        /// - Destination key (exact match)
        /// - Departure date (on or after specified date)
        /// - Passenger count (minimum capacity)
        /// </remarks>
        Task<List<Trip>> GetFilteredTripsAsync(TripFilterRequest filter);

        Task<List<Trip>> FindNearbyTripsAsync(TripFilterRequest filter, bool excludeExact = false);

    }
}
