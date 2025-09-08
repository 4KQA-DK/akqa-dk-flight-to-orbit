// /DTO/BookingDtos.cs
using System.ComponentModel.DataAnnotations;
using UmbracoProject.Models;

namespace UmbracoProject.DTO
{
    public class PassengerRequest
    {
        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;

        [Required] public string Email { get; set; } = null!;
        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; } 
    }

    public class CreateBookingRequest
    {
        [Required] public Guid TripId { get; set; }

        [MinLength(1, ErrorMessage = "At least one passenger is required.")]
        public List<PassengerRequest> Passengers { get; set; } = new();

    }

    public class CreateBookingResponse
    {
        public Guid BookingId { get; set; }
        public Guid TripId { get; set; }
        public int PassengerCount { get; set; }
        public double TotalPrice { get; set; }
        public DateTime BookedAtUtc { get; set; }
    }

    public class GetBookingResponse
    {
        public Guid BookingId { get; set; }
        public Guid TripId { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }

        public List<PassengerItem> Passengers { get; set; } = new();

        public class PassengerItem
        {
            public Guid PassengerId { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; }
            public DateOnly BirthDate { get; set; }
            public Gender Gender { get; set; }
        }
    }
}
