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

        Task UpdateAvaliableSeatsCountAsync(Guid id, int avalialbeseats);

        Task<List<Trip>> GetFilteredTripsAsync(TripFilterRequest filter);

        Task<List<Trip>> FindNearbyTripsAsync(TripFilterRequest filter);

        Task<bool> HasOverlappingTripByDateAsync(Guid rocketKey, DateOnly startDate, DateOnly endDate, int turnaroundDays);


    }
}
