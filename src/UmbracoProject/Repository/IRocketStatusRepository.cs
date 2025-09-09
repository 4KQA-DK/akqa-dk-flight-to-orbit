using UmbracoProject.Models;

namespace UmbracoProject.Repository
{
    public interface IRocketStatusRepository
    {
        Task<RocketStatus> GetAsync(Guid rocketKey);

        Task<List<RocketStatus>> GetAllAsync();
        Task<RocketStatus> CreateAsync(RocketStatus row);
        Task UpdateAsync(RocketStatus row);
        Task DeleteAsync(Guid rocketKey);
    }
}
