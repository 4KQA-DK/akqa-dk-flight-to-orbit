using System.ComponentModel.DataAnnotations;
using UmbracoProject.Models;

namespace UmbracoProject.DTO
{
    public class CreateTripRequest
    {
        public Guid RocketKey { get; set; }     // Umbraco Key (GUID)

        public Guid DestinationKey { get; set; }

        [Required]
        public DateTime DepartureUtc { get; set; }

        [Required]
        public DateTime ArrivalUtc { get; set; }

        [Range(0, int.MaxValue)]
        public int PassengerCount { get; set; }

        [Range(0, double.MaxValue)]
        public double Price { get; set; }
    }
}
