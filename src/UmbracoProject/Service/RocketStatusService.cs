using UmbracoProject.Models;
using UmbracoProject.Repository;
using UmbracoProject.DTO;
using Umbraco.Cms.Core.Services;

namespace UmbracoProject.Service
{
    public class RocketStatusService:IRocketStatusService
    {
        private readonly IContentService _contentService;
        private readonly IRocketStatusRepository _repository;

        public RocketStatusService(IContentService contentService, IRocketStatusRepository repository)
        {
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<RocketStatus> GetAsync(Guid rocketKey)
        {
            if (rocketKey == Guid.Empty) throw new ArgumentException("Rocket key is required.", nameof(rocketKey));

            var current = await _repository.GetAsync(rocketKey);
            return current ?? RocketStatus.Unknown;
        }

        /// <inheritdoc />
        public async Task SetAsync(Guid rocketKey, RocketStatus status, string? note = null, string? reason = null)
        {
            ValidateRocketExists(rocketKey);
            // Persist snapshot (note and reason are optional; repository may ignore reason if you didn't add history)
            await _repository.UpsertAsync(rocketKey, status, note, reason);
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
