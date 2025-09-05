using UmbracoProject.Models;

namespace UmbracoProject.Repository
{
    public interface IRocketStatusRepository
    {
        Task<RocketStatus?> GetAsync(Guid rocketKey);
        Task <RocketStatus> UpsertAsync(Guid rocketKey, RocketStatusCode status);
    }
}
