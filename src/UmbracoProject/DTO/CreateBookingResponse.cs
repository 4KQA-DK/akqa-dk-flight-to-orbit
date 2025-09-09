namespace UmbracoProject.DTO
{
    public class CreateBookingResponse
    {
        public Guid BookingId { get; set; }
        public Guid TripId { get; set; }
        public int PassengerCount { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookedAtUtc { get; set; }
    }
}
