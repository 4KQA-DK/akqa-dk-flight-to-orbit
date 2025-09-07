namespace UmbracoProject.DTO
{
    public class TripFilterResponse
    {
        
        public List<GetTripResponse> ExactMatches { get; set; } = new();

        
        public List<GetTripResponse> NearbyTrips { get; set; } = new();

        
        public DateOnly? SearchedDate { get; set; }

        
        public bool HasExactMatches { get; set; }

        
        public int TotalTrips => ExactMatches.Count + NearbyTrips.Count;

        public string? Message { get; set; }
    }
}
