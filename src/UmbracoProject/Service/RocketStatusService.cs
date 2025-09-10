using Umbraco.Cms.Core.Services;
using UmbracoProject.DTO;
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
            _contentService = contentService;
            _repository = repository;
        }

        public async Task<RocketStatus> TryGetAsync(Guid rocketKey)
        {
            ValidateRocketExists(rocketKey);

            try
            {
                return await _repository.GetAsync(rocketKey);
            }
            catch (ArgumentException)
            {

                throw new ArgumentException("RocketStatus not found in db");
            }
             
        }

        public Task<List<RocketStatus>> GetAllAsync() => _repository.GetAllAsync();

        public async Task<RocketStatus> CreateStatusOnPublishAsync(Guid rocketKey)
        {
            ValidateRocketExists(rocketKey);

            var current = await _repository.GetAsync(rocketKey);
            if (current != null) return current;

            var now = DateTime.UtcNow;
            var created = new RocketStatus
            {
                rocketKey = rocketKey,
                rocketStatus = RocketStatusCode.Idle,
                lastUpdatedUtc = now
            };
            return await _repository.CreateAsync(created);
        }

        public async Task UpdateAsync(Guid rocketKey, RocketStatusCode newStatus)
        {
            ValidateRocketExists(rocketKey);

            var row = await _repository.GetAsync(rocketKey)
                      ?? throw new InvalidOperationException("RocketStatus row does not exist.");

            if (row.rocketStatus == newStatus) return;

            var from = row.rocketStatus;
            var allowed = false;

            if (from == RocketStatusCode.Idle)
            {
                allowed = newStatus == RocketStatusCode.Reserved
                       || newStatus == RocketStatusCode.Maintenance;
            }
            else if (from == RocketStatusCode.Reserved)
            {
                allowed = newStatus == RocketStatusCode.InFlight
                       || newStatus == RocketStatusCode.Idle;
            }
            else if (from == RocketStatusCode.InFlight)
            {
                allowed = newStatus == RocketStatusCode.Idle
                       || newStatus == RocketStatusCode.Maintenance;
            }
            else if (from == RocketStatusCode.Maintenance)
            {
                allowed = newStatus == RocketStatusCode.Idle;
            }

            if (!allowed)
                throw new InvalidOperationException($"Invalid rocket status change: {from} → {newStatus}.");

            row.rocketStatus = newStatus;
            row.lastUpdatedUtc = DateTime.UtcNow;
            await _repository.UpdateAsync(row);
        }

        public async Task<GetRocketResponse> GetRocketMetadataAsync(Guid rocketKey)
        {
            ValidateRocketExists(rocketKey);

            var content = _contentService.GetById(rocketKey)
                          ?? throw new ArgumentException("Rocket not found.");

            var rocketStatusRow = await _repository.GetAsync(rocketKey);
            var status = rocketStatusRow?.rocketStatus.ToString() ?? RocketStatusCode.Idle.ToString();

            var modelValue = content.GetValue("model")?.ToString();
            string modelName = "(unknown model)";
            int capacity = 0;

            if (!string.IsNullOrEmpty(modelValue))
            {
                Guid modelKey;
                if (modelValue.StartsWith("umb://document/") && Guid.TryParse(modelValue.Replace("umb://document/", ""), out var udiGuid))
                {
                    modelKey = udiGuid;
                }
                else if (Guid.TryParse(modelValue, out var directGuid))
                {
                    modelKey = directGuid;
                }
                else
                {
                    modelKey = Guid.Empty;
                }

                if (modelKey != Guid.Empty)
                {
                    var modelContent = _contentService.GetById(modelKey);
                    if (modelContent != null)
                    {
                        modelName = modelContent.Name;
                        capacity = modelContent.GetValue<int>("capacity");
                    }
                }
            }

            return new GetRocketResponse
            {
                RocketKey = rocketKey,
                RocketName = content.Name,
                Model = modelName,
                Capacity = capacity,
                Status = status
            };
        }


        public Task DeleteAsync(Guid rocketKey) => _repository.DeleteAsync(rocketKey);


        private void ValidateRocketExists(Guid rocketKey)
        {
            if (rocketKey == Guid.Empty) throw new ArgumentException("Rocket key is required.", nameof(rocketKey));
            var content = _contentService.GetById(rocketKey) ?? throw new ArgumentException("Rocket not found.");
            if (content.Trashed) throw new ArgumentException("Rocket is in recycle bin.");
        }
    }
}
