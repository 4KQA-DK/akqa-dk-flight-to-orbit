using UmbracoProject.DTO;
using UmbracoProject.Models;
using UmbracoProject.Pagination;

namespace UmbracoProject.Service
{
    public interface ITripService
    {
        Task<TripFilterResponse> GetFilteredTripsAsync(TripFilterRequest filter, PageParameters page);

    }
}
