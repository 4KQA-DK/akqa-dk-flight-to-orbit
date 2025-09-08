namespace UmbracoProject.Service
{
    public interface IPriceCalculatorService
    {
        decimal CalculatePrice(Guid rocketKey, Guid destinationKey, decimal? discountPercent);
    }
}
