using System.ComponentModel.DataAnnotations;

namespace UmbracoProject.DTO
{
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "TripId is required")]
        public Guid TripId { get; set; }

        [MinLength(1, ErrorMessage = "At least one passenger is required.")]
        public List<PassengerRequest> Passengers { get; set; } = new();

    }
}
