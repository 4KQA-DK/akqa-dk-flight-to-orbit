using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbracoProject.CustomDatabase.Models
{
    [TableName("Passenger")]
    [PrimaryKey("passengerId", AutoIncrement = false)]
    public class Passenger
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("passengerId")]
        public Guid passengerId { get; set; }

        [Column("BookingId")]
        public Guid bookingId { get; set; }

        [Column("firstName")]
        public required string firstName { get; set; }

        [Column("lastName")]
        public required string lastName { get; set; }

        [Column("birthDate")]
        public DateOnly? birthDate { get; set; }

        [Column("gender")]
        public Gender gender { get; set; }

        // SeatNumber?
        // PassportNumber?
        // Nationality?

    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2,
    }
}
