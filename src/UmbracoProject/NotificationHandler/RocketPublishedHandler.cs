using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using UmbracoProject.Models;
using UmbracoProject.Service;
namespace UmbracoProject.NotificationHandler
{
    public sealed class RocketPublishedHandler
    : INotificationAsyncHandler<ContentPublishedNotification>
    {
        private const string RocketDocTypeAlias = "rocket"; 
        private readonly IRocketStatusService _statusService;
        private readonly ILogger<RocketPublishedHandler> _logger;

        public RocketPublishedHandler(
            IRocketStatusService statusService,
            ILogger<RocketPublishedHandler> logger)
        {
            _statusService = statusService;
            _logger = logger;
        }

        public async Task HandleAsync(ContentPublishedNotification notification, CancellationToken ct)
        {
            foreach (var content in notification.PublishedEntities)
            {
                if (!string.Equals(content.ContentType.Alias, RocketDocTypeAlias, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    await _statusService.CreateStatusOnPublishAsync(content.Key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize RocketStatus for rocket {RocketKey}", content.Key);
                }
            }
        }
    }
}
