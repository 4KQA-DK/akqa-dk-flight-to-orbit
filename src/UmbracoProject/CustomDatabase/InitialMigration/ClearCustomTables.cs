using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace UmbracoProject.CustomDatabase.Migrations
{
    public class ClearCustomTables : AsyncMigrationBase
    {
        public ClearCustomTables(IMigrationContext context) : base(context) { }

        protected override async Task MigrateAsync()
        {

            if (TableExists("Passenger"))
                Database.Execute("DELETE FROM [Passenger]");

            if (TableExists("Booking"))
                Database.Execute("DELETE FROM [Booking]");

            if (TableExists("Trip"))
                Database.Execute("DELETE FROM [Trip]");

            if (TableExists("RocketStatus"))
                Database.Execute("DELETE FROM [RocketStatus]");

            await Task.CompletedTask;
        }
    }
}
