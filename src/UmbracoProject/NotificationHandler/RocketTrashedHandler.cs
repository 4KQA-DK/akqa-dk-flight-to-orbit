using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using UmbracoProject.Models;
using UmbracoProject.Service;

namespace UmbracoProject.NotificationHandler
{
    public sealed class RocketTrashedHandler : INotificationHandler<ContentMovedToRecycleBinNotification>
    {
        private readonly IRocketStatusService _rocketStatusService;
        public RocketTrashedHandler(IRocketStatusService rocketStatusService)
        {
            _rocketStatusService = rocketStatusService;
        }

        public async void Handle(ContentMovedToRecycleBinNotification n)
        {
            foreach (var c in n.MoveInfoCollection.Select(x => x.Entity))
            {
                if (IsRocket(c))
                {
                    await _rocketStatusService.UpdateAsync(c.Key, RocketStatusCode.Trashed);
                }
            }
        }

        private static bool IsRocket(IContent c) => c.ContentType.Alias == "rocket";
    }
}
