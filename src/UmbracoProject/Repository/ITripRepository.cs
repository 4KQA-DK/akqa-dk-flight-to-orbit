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

    }
}
