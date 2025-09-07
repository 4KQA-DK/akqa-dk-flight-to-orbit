using System.Threading.Tasks;
using UmbracoProject.Models;

namespace UmbracoProject.Service
{
    public interface IRocketStatusService
    {
        Task<RocketStatus?> TryGetAsync(Guid rocketKey);                       
        Task<RocketStatus> CreateStatusOnPublishAsync(Guid rocketKey);              
        Task UpdateAsync(Guid rocketKey, RocketStatusCode newStatus); 

        Task DeleteAsync(Guid rocketKey);
    }
}
