using UmbracoProject.Models;

namespace UmbracoProject.DTO
{
    public class GetTripResponse
    {
        public Guid TripId { get; set; }
        public string RocketName { get; set; } = string.Empty;

        public Guid RocketKey { get; set; }

        public string DestinationName { get; set; } = string.Empty;

        public Guid DestinationKey { get; set; }

        public DateTime DepartureUtc { get; set; }

        public DateTime ArrivalUtc { get; set; }

        public int EstimatedTravelDays { get; set; }


        public int PassengerCount { get; set; }

        public decimal Price { get; set; }

        public TripStatus TripStatus { get; set; }
    }
}
