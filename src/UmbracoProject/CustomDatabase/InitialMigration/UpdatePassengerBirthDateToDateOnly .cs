using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

public class UpdatePassengerBirthDateToDateOnlySimple : AsyncMigrationBase
{
    public UpdatePassengerBirthDateToDateOnlySimple(IMigrationContext context) : base(context)
    {
    }

    protected override async Task MigrateAsync()
    {
        if (TableExists("Passenger"))
        {
            Delete.Table("Passenger").Do();
            Create.Table<PassengerSchemaV2>().Do();
        }
        else
        {
            Create.Table<PassengerSchemaV2>().Do();
        }

    }

    [TableName("Passenger")]
    [PrimaryKey("passengerId", AutoIncrement = false)]
    [ExplicitColumns]
    public class PassengerSchemaV2
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("passengerId")]
        public Guid passengerId { get; set; }

        [Column("BookingId")]
        public Guid bookingId { get; set; }

        [Column("firstName")]
        public string firstName { get; set; } = null!;

        [Column("lastName")]
        public string lastName { get; set; } = null!;

        [Column("birthDate")]
        public DateTime birthDate { get; set; }

        [Column("gender")]
        public int gender { get; set; }

    }
}