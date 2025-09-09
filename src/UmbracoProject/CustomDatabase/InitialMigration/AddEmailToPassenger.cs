using Microsoft.AspNetCore.Http.HttpResults;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;


namespace UmbracoProject.CustomDatabase.InitialMigration
{
    public class AddEmailToPassenger : AsyncMigrationBase
    {
        public AddEmailToPassenger(IMigrationContext ctx) : base(ctx) { }

        protected override async Task MigrateAsync()
        {
            if (!TableExists("Passenger"))
            {
                Create.Table<PassengerSchemaV3>().Do();
            }


            if (ColumnExists("Passenger", "email"))
            {
                return;
            }

            Alter.Table("Passenger")
               .AddColumn("email")
               .AsString(320)
               .Do();


        }

    }

    [TableName("Passenger")]
    [PrimaryKey("passengerId", AutoIncrement = false)]
    [ExplicitColumns]
    public class PassengerSchemaV3
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

        [Column("email")]
        public string email { get; set; } = null!;

        [Column("birthDate")]
        public DateTime birthDate { get; set; }

        [Column("gender")]
        public int gender { get; set; }

    }
}
