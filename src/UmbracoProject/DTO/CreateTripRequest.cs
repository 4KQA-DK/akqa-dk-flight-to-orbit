using System.ComponentModel.DataAnnotations;

namespace UmbracoProject.DTO
{
    public class CreateTripRequest
    {
        [Required(ErrorMessage = "RocketKey is required")]
        public Guid RocketKey { get; set; }

        [Required(ErrorMessage = "DestinationKey is required")]
        public Guid DestinationKey { get; set; }

        [Required]
        public DateTime DepartureUtc { get; set; }

        [Required]
        public DateTime ArrivalUtc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PassengerCount must be at least 1")]
        public int AvalivableSeats { get; set; }

        [Range(0, 100, ErrorMessage = "DiscountPercent must be between 0 and 100")]
        public decimal? DiscountPercent { get; set; } = 0;
    }


}
