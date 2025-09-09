using Umbraco.Cms.Core.Services;
using UmbracoProject.Service;
public class PriceCalculatorService : IPriceCalculatorService
{
    private readonly IContentService _content;
    public PriceCalculatorService(IContentService content) => _content = content;


    // basepris for at flyve til en destination
    private static readonly Dictionary<Guid, decimal> BasePricePerDestination = new()
    {
        { Guid.Parse("057474e6-8508-4bee-861c-cdcf8ca0b9c5"), 3000000m}, // Mercury
        { Guid.Parse("ce20a134-d61e-4733-ac5f-390bb3d40109"), 2000000m}, // Venus
        { Guid.Parse("6436e764-31be-4c93-834c-67b70d3c0834"), 1000000m}, // Moon
        { Guid.Parse("84abe17d-2408-4023-b02e-4ff6410ae3fc"), 3400000m}, //Mars
        { Guid.Parse("c263992b-4812-4923-b06d-5d0355dde7a6"), 8200000m}, //Jupiter
        { Guid.Parse("c671ea8f-dbc6-46fe-91eb-d3449bd838ca"), 10000000m}, //Saturn
        { Guid.Parse("652c13ce-9adb-4868-a00e-a73d2915b27b"), 12000000m}, //Uranus
        { Guid.Parse("6052d089-3419-4baf-9232-981b7db0cc64"), 15000000m} //Neptune
        
    };

    // multiplikator pr. model (langsommere = billigere, hurtigere = dyrere)
    private static readonly Dictionary<Guid, decimal> ModelMultipliers = new()
    {
        { Guid.Parse("9912c873-4f05-4464-a7ff-e511d283a57c"), 1.0m},  //Polar Starship
        { Guid.Parse("942ded54-a2f4-438c-a92e-973801eeeb69"), 1.3m}, // Falcon X
        { Guid.Parse("eaf72b64-e1fa-4ebf-8755-b8d9837e5e97"), 1.6m}  // Aurora
    };

    public decimal CalculatePrice(Guid rocketKey, Guid destinationKey, decimal? discountPercent)
    {
        if (!BasePricePerDestination.TryGetValue(destinationKey, out var basePrice))
            throw new KeyNotFoundException($"No base price for destination {destinationKey}");

        var rocket = _content.GetById(rocketKey)
                     ?? throw new KeyNotFoundException($"Rocket {rocketKey} not found");

        var raw = rocket.GetValue<string>("model");
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidOperationException($"Rocket {rocketKey} has no model assigned");

        var guidPart = raw.Split('/').Last();   
        var modelId = Guid.Parse(guidPart);

        if (!ModelMultipliers.TryGetValue(modelId, out var multiplier))
            throw new KeyNotFoundException($"No multiplier for model {modelId}");

        var price = basePrice * multiplier;

        if (discountPercent.HasValue)
        {
            price *= (100m - discountPercent.Value) / 100m;
        }

        return Math.Round(price, 2, MidpointRounding.AwayFromZero);
    }
}
