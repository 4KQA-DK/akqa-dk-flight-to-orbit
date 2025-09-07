using System.ComponentModel.DataAnnotations;

namespace UmbracoProject.DTO
{
    public enum TripSortBy
    {
        Departure = 0,   // default tab = All Trips
        Price = 1,   // Cheapest Trips
        Duration = 2    // Fastest Trips
    }
    public class TripFilterRequest
    {
        public DateOnly? DepartureDate { get; set; }
        public Guid? DestinationKey { get; set; }

        [Range(1, 10, ErrorMessage = "Passenger count must be between 1 and 10.")]
        public int? PassengerCount { get; set; }

        public TripSortBy SortBy { get; set; } = TripSortBy.Departure;
    }
}
