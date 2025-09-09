using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
namespace UmbracoProject.Models
{
    [TableName("Booking")]
    [PrimaryKey("bookingId", AutoIncrement = false)]
    public class Booking
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("bookingId")]
        public Guid bookingId { get; set; }

        public Guid tripId { get; set; }

        public decimal price { get; set; }

        public DateTime date { get; set; }

    }
}
