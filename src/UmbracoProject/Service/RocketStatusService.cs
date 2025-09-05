using Umbraco.Cms.Core.Services;
using UmbracoProject.Models;
using UmbracoProject.Repository;

namespace UmbracoProject.Service
{
    public class RocketStatusService : IRocketStatusService
    {
        private readonly IContentService _contentService;
        private readonly IRocketStatusRepository _repository;

        public RocketStatusService(IContentService contentService, IRocketStatusRepository repository)
        {
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }


        public async Task<RocketStatus> GetRocketStatusAsync(Guid rocketKey)
        {
            ValidateRocketExists(rocketKey);

            var row = await _repository.GetAsync(rocketKey);
            if (row != null)
            {
                return row;
            }
            else
            {
                return await _repository.UpsertAsync(rocketKey, RocketStatusCode.Idle);
            }
        }

        public async Task SetAsync(Guid rocketKey, RocketStatusCode status)
        {
            ValidateRocketExists(rocketKey);



            await _repository.UpsertAsync(rocketKey, status);
        }

        private void ValidateRocketExists(Guid rocketKey)
        {
            if (rocketKey == Guid.Empty) throw new ArgumentException("Rocket key is required.", nameof(rocketKey));

            var content = _contentService.GetById(rocketKey);
            if (content is null) throw new ArgumentException("Rocket not found.");
            if (content.Trashed) throw new ArgumentException("Rocket is in recycle bin.");
        }
    }
}
