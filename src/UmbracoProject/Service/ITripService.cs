using UmbracoProject.Models;
using UmbracoProject.DTO;

namespace UmbracoProject.Service
{
    public interface ITripService
    {
       Task<TripFilterResponse> GetFilteredTripsAsync(TripFilterRequest filter, int pageNumberas);

    }
}
