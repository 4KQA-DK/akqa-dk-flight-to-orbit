using System;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using UmbracoProject.Service;

namespace UmbracoProject.NotificationHandler
{
    public sealed class RocketDeletedHandler : INotificationHandler<ContentDeletedNotification>
    {
        private readonly IRocketStatusService _rocketStatusService;

        public RocketDeletedHandler(IRocketStatusService rocketStatusService)
        {
            _rocketStatusService = rocketStatusService;
        }

        public async void Handle(ContentDeletedNotification notification)
        {
            foreach (var content in notification.DeletedEntities.Where(IsRocket))
            {
                await _rocketStatusService.DeleteAsync(content.Key);
            }
        }

        private static bool IsRocket(IContent content) => content.ContentType.Alias == "rocket";
    }
}
