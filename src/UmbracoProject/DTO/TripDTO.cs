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
        public Guid? DestinationKey { get; set; }
        public int? PassengerCount { get; set; }

        public int NearbyDaysRange { get; set; } = 10;

        public int NearbyCount { get; set; } = 5;

    }

    public class TripFilterResponse
    {
        /// <summary>
        /// Trips that exactly match the search criteria
        /// </summary>
        public List<GetTripResponse> ExactMatches { get; set; } = new();

        /// <summary>
        /// Trips that are close to the search date but don't exactly match
        /// </summary>
        public List<GetTripResponse> NearbyTrips { get; set; } = new();

        /// <summary>
        /// The date that was searched for
        /// </summary>
        public DateOnly? SearchedDate { get; set; }

        /// <summary>
        /// Whether any trips were found that exactly match the criteria
        /// </summary>
        public bool HasExactMatches { get; set; }

        /// <summary>
        /// Total number of trips returned (exact + nearby)
        /// </summary>
        public int TotalTrips => ExactMatches.Count + NearbyTrips.Count;

        /// <summary>
        /// Message for frontend to display if needed
        /// </summary>
        public string? Message { get; set; }
    }


}
