using Umbraco.Cms.Core.Services;
using UmbracoProject.Service;
namespace UmbracoProject.Service
{
    public class TravelTimeEstimator:ITravelTimeEstimator
    {
        private readonly IContentService _content;
        public TravelTimeEstimator(IContentService content) => _content = content;

        private static readonly Dictionary<Guid, TimeSpan> BaseDurationPerDestination = new()
        {
            { Guid.Parse("057474e6-8508-4bee-861c-cdcf8ca0b9c5"), TimeSpan.FromDays(48.3) },  // Mercury
            { Guid.Parse("ce20a134-d61e-4733-ac5f-390bb3d40109"), TimeSpan.FromDays(84.56) },  // Venus
            { Guid.Parse("6436e764-31be-4c93-834c-67b70d3c0834"), TimeSpan.FromDays(14.25) },  // Moon
            { Guid.Parse("84abe17d-2408-4023-b02e-4ff6410ae3fc"), TimeSpan.FromDays(112.34) },  // Mars
            { Guid.Parse("c263992b-4812-4923-b06d-5d0355dde7a6"), TimeSpan.FromDays(130.75) },  // Jupiter
            { Guid.Parse("c671ea8f-dbc6-46fe-91eb-d3449bd838ca"), TimeSpan.FromDays(201.40) },  // Saturn
            { Guid.Parse("652c13ce-9adb-4868-a00e-a73d2915b27b"), TimeSpan.FromDays(252.80) },  // Uranus
            { Guid.Parse("6052d089-3419-4baf-9232-981b7db0cc64"), TimeSpan.FromDays(395.50) }   // Neptune
        };

        private static readonly Dictionary<Guid, double> ModelSpeedMultipliers = new()
        {
            { Guid.Parse("9912c873-4f05-4464-a7ff-e511d283a57c"), 1.00}, // Polar Starship (baseline)
            { Guid.Parse("942ded54-a2f4-438c-a92e-973801eeeb69"), 0.80}, // Falcon X (20% faster)
            { Guid.Parse("eaf72b64-e1fa-4ebf-8755-b8d9837e5e97"), 0.60}  // Aurora   (40% faster)
        };

        public TimeSpan EstimateTravelTime(Guid rocketKey, Guid destinationKey)
        {
            if (!BaseDurationPerDestination.TryGetValue(destinationKey, out var baseDuration))
            {
                throw new KeyNotFoundException($"No base duration for destination {destinationKey}");
            }
                

            var rocket = _content.GetById(rocketKey) ?? throw new KeyNotFoundException($"Rocket {rocketKey} not found");

            var udiString = rocket.GetValue<string>("model");

            if (string.IsNullOrWhiteSpace(udiString))
            {
                throw new InvalidOperationException($"Rocket {rocketKey} has no model assigned");
            }
                

            var modelGuidPart = udiString.Split('/').Last();
            var modelId = Guid.Parse(modelGuidPart);

            if (!ModelSpeedMultipliers.TryGetValue(modelId, out var mul))
            {
                throw new KeyNotFoundException($"No speed multiplier for model {modelId}");
            }

            var adjustedDays = baseDuration.TotalDays * (double)mul;
            return TimeSpan.FromDays(adjustedDays);
        }

    }
}


