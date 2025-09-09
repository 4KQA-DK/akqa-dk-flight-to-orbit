namespace UmbracoProject.Service
{
    public interface ITravelTimeEstimator
    {
        TimeSpan EstimateTravelTime(Guid rocketKey, Guid destinationKey);
    }
}
