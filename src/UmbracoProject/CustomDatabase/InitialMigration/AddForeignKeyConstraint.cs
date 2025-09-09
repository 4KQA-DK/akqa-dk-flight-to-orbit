using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbracoProject.CustomDatabase.InitialMigration
{
    public class AddForeignKeyConstraint : AsyncMigrationBase
    {
        public AddForeignKeyConstraint(IMigrationContext ctx) : base(ctx) { }

        protected override async Task MigrateAsync()
        {
            if (TableExists("Booking") && TableExists("Trip") && !ForeignKeyExists("FK_Booking_Trip"))
            {
                Create.ForeignKey("FK_Booking_Trip")
                    .FromTable("Booking").ForeignColumn("tripId")
                    .ToTable("Trip").PrimaryColumn("tripId")
                    .Do();
            }

            if (TableExists("Passenger") && TableExists("Booking") && !ForeignKeyExists("FK_Passenger_Booking"))
            {
                Create.ForeignKey("FK_Passenger_Booking")
                    .FromTable("Passenger").ForeignColumn("bookingId")
                    .ToTable("Booking").PrimaryColumn("bookingId")
                    .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                    .Do();
            }

            await Task.CompletedTask;
        }


        private bool ForeignKeyExists(string fkName)
        {
            const string sql = "SELECT 1 FROM sys.foreign_keys WHERE name = @0";
            return Database.ExecuteScalar<int?>(sql, fkName).HasValue;
        }
    }
}
