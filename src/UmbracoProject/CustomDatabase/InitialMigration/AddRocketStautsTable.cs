using Microsoft.AspNetCore.Http.HttpResults;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using static UmbracoProject.CustomDatabase.InitialMigration.AddTransactionalTables.AddTransactionalTables;

public class AddRocketStatusTable : AsyncMigrationBase
{
    public AddRocketStatusTable(IMigrationContext ctx) : base(ctx) { }

    protected override async Task MigrateAsync()
    {
        if (!TableExists("RocketStatus"))
        {
            Create.Table<RocketStatusSchema>().Do();
        }
    }

    [TableName("RocketStatus")]
    [PrimaryKey("rocketKey", AutoIncrement = false)]
    [ExplicitColumns]
    public class RocketStatusSchema
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("rocketKey")]
        public Guid rocketKey { get; set; }

        [Column("currentStatus")]
        public int currentStatus { get; set; }

        [Column("lastUpdatedUtc")]
        public DateTime lastUpdatedUtc { get; set; }
    }
}
