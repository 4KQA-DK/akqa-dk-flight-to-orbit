using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbracoProject.Models
{
    public enum RocketStatusCode
    {
        Idle = 0,
        Reserved = 1,
        InFlight = 2,
        Maintenance = 3,
        Trashed = 4,
    }

    [TableName("RocketStatus")]
    [PrimaryKey("rocketKey", AutoIncrement = false)]
    public class RocketStatus
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("rocketKey")]
        public Guid rocketKey { get; set; }

        [Column("currentStatus")]
        public RocketStatusCode rocketStatus { get; set; }

        [Column("lastUpdatedUtc")]
        public DateTime lastUpdatedUtc { get; set; }
    }
}