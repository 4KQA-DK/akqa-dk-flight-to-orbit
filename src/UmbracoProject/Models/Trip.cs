using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbracoProject.Models
{
    [TableName("Trip")]
    [PrimaryKey("tripId", AutoIncrement = false)]
    public class Trip
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("tripId")]
        public Guid tripId { get; set; }

        // Store Umbraco content keys (GUIDs)
        [Column("destinationKey")]
        public Guid destinationKey { get; set; }

        [Column("rocketKey")]
        public Guid rocketKey { get; set; }

        [Column("departureUtc")]
        public DateTime departureUtc { get; set; }

        [Column("arrivalUtc")]
        public DateTime arrivalUtc { get; set; }

        [Column("passengerCount")]
        public int passengerCount { get; set; }

        [Column("price")]
        public double price { get; set; }

        [Column("tripStatus")]
        public TripStatus tripStatus { get; set; }
    }

    public enum TripStatus
    {
        Schedueled = 0,
        Ongoing = 1,
        Completed = 2,
        Cancelled = 3
    }


}

