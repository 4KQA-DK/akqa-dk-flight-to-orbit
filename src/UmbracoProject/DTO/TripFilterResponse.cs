namespace UmbracoProject.DTO
{
    public class TripFilterResponse
    {
        
        public List<GetTripResponse> ExactMatches { get; set; } = new();

        
        public List<GetTripResponse> NearbyTrips { get; set; } = new();

        
        public DateOnly? SearchedDate { get; set; }

        
        public bool HasExactMatches { get; set; }

        public int TotalTrips { get; set; }      
        public int PageNumber { get; set; }      
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public string? Message { get; set; }


    }
}
