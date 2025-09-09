using UmbracoProject.Models;

namespace UmbracoProject.DTO
{
    public class GetBookingResponse
    {
        public Guid BookingId { get; set; }
        public Guid TripId { get; set; }
        public decimal Price { get; set; }
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
