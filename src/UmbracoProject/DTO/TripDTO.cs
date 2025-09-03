using System.ComponentModel.DataAnnotations;
using UmbracoProject.Models;

namespace UmbracoProject.DTO
{
    public class CreateTripRequest
    {
        public Guid RocketKey { get; set; }

        public Guid DestinationKey { get; set; }

        [Required]
        public DateTime DepartureUtc { get; set; }

        [Required]
        public DateTime ArrivalUtc { get; set; }

        public int PassengerCount { get; set; }

        [Range(0, double.MaxValue)]
        public double Price { get; set; }
    }

    public class GetTripResponse
    {
        public Guid TripId { get; set; }
        public string RocketName { get; set; } = string.Empty;

        public string DestinationName { get; set; } = string.Empty;

        public DateTime DepartureUtc { get; set; }

        public DateTime ArrivalUtc { get; set; }

        public int PassengerCount { get; set; }

        public double Price { get; set; }

        public TripStatus TripStatus { get; set; }
    }

    public class TripFilterRequest
    {
        public DateOnly? DepartureDate { get; set; }
        public DateOnly? ArrivalDate { get; set; }
        public Guid? DestinationKey { get; set; }

        public int? PassengerCount { get; set; }

        public int NearbyDaysRange { get; set; } = 10;

        public int NearbyCount { get; set; } = 5;
    }
}
