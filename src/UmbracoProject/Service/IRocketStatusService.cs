using System.Threading.Tasks;
using UmbracoProject.DTO;
using UmbracoProject.Models;

namespace UmbracoProject.Service
{
    public interface IRocketStatusService
    {
        Task<RocketStatus> TryGetAsync(Guid rocketKey);        
        
        Task<List<RocketStatus>> GetAllAsync();
        Task<RocketStatus> CreateStatusOnPublishAsync(Guid rocketKey);              
        Task UpdateAsync(Guid rocketKey, RocketStatusCode newStatus); 

        Task DeleteAsync(Guid rocketKey);
        
        Task<GetRocketResponse> GetRocketMetadataAsync(Guid rocketKey);
    }
}
