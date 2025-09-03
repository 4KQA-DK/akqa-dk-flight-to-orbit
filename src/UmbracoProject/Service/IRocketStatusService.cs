using UmbracoProject.Models;

namespace UmbracoProject.Service
{
    public interface IRocketStatusService
    {
        /// <summary>Gets the current status; returns <see cref="RocketStatus.Unknown"/> if none is stored.</summary>
        Task<RocketStatus> GetAsync(Guid rocketKey);

        /// <summary>Sets the rocket's status (validates the Umbraco content exists and isn't trashed).</summary>
        Task SetAsync(Guid rocketKey, RocketStatus status, string? note = null, string? reason = null);

    }
}
