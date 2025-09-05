using System.Threading.Tasks;
using UmbracoProject.Models;

namespace UmbracoProject.Service
{
    public interface IRocketStatusService
    {
        Task<RocketStatus?> GetRocketStatusAsync(Guid rocketKey);
        Task SetAsync(Guid rocketKey, RocketStatusCode status);
    }
}
