using UmbracoProject.Models;
namespace UmbracoProject.Repository
{
    public interface ITripRepository
    {
        Task CreateTripAsync(Trip trip);
    }
}
