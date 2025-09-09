using UmbracoProject.Models;

namespace UmbracoProject.DTO
{
    public class GetTripResponse
    {
        public Guid TripId { get; set; }
        public string RocketName { get; set; } = string.Empty;

        public string DestinationName { get; set; } = string.Empty;

        public DateTime DepartureUtc { get; set; }

        public DateTime ArrivalUtc { get; set; }

        public int EstimatedTravelDays { get; set; }


        public int PassengerCount { get; set; }

        public decimal Price { get; set; }

        public TripStatus TripStatus { get; set; }
    }
}
