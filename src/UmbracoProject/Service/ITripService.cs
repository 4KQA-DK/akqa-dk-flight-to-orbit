using UmbracoProject.Models;
using UmbracoProject.DTO;

namespace UmbracoProject.Service
{
    public interface ITripService
    {
        Task<Guid> CreateTripAsync(CreateTripRequest request);
    }
}
