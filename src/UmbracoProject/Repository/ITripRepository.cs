using UmbracoProject.DTO;
using UmbracoProject.Models;
using UmbracoProject.Pagination;

namespace UmbracoProject.Repository
{
    public interface ITripRepository
    {
        Task CreateTripAsync(Trip trip);
        Task<Trip?> GetTripByIdAsync(Guid tripId);

        Task<List<Trip>> GetAllTripsAsync();

        Task<bool> UpdateStatusAsync(Guid id, TripStatus status);

        Task UpdateAvaliableSeatsCountAsync(Guid id, int avalialbeseats);

        Task<PagedList<Trip>> GetFilteredTripsAsync(TripFilterRequest filter, PageParameters page);

        Task<List<Trip>> FindNearbyTripsAsync(TripFilterRequest filter);

        Task<bool> HasOverlappingTripByDateAsync(Guid rocketKey, DateOnly startDate, DateOnly endDate, int turnaroundDays);

        Task<int> UpdateTripStatusBackgroundServiceAsync();

    }
}
