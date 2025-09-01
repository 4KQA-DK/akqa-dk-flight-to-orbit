using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace UmbracoProject.CustomDatabase.InitialMigration.AddTransactionalTables
{
    public class TransactionalTablesComposer : ComponentComposer<TransactionalTablesComponent>
    {

    }

    public class TransactionalTablesComponent : IAsyncComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public TransactionalTablesComponent(
            ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _coreScopeProvider = coreScopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public async Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            Console.WriteLine("TransactionalTablesComponent initializing..."); // Add this
            

            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }

            var migrationPlan = new MigrationPlan("TransactionalTables");
            migrationPlan.From(string.Empty)
                .To<AddTransactionalTables>("transactional-tables-db");

            var upgrader = new Upgrader(migrationPlan);
            await upgrader.ExecuteAsync(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class AddTransactionalTables : AsyncMigrationBase
    {
        public AddTransactionalTables(IMigrationContext context) : base(context)
        {
        }

        protected override async Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", "AddTransactionalTables");

            if (!TableExists("Booking"))
            {
                Create.Table<BookingSchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "Booking");
            }

            if (!TableExists("Passenger"))
            {
                Create.Table<PassengerSchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "Passenger");
            }

            if (!TableExists("Trip"))
            {
                Create.Table<TripSchema>().Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", "Trip");
            }

            await Task.CompletedTask;
        }

        // Immutable schema snapshot classes for migration only
        [TableName("Booking")]
        [PrimaryKey("bookingId", AutoIncrement = false)]
        [ExplicitColumns]
        public class BookingSchema
        {
            [PrimaryKeyColumn(AutoIncrement = false)]
            [Column("bookingId")]
            public Guid bookingId { get; set; }

            [Column("tripId")]
            public Guid tripId { get; set; }

            [Column("price")]
            public double price { get; set; }

            [Column("date")]
            public DateTime? date { get; set; }
        }

        [TableName("Passenger")]
        [PrimaryKey("passengerId", AutoIncrement = false)]
        [ExplicitColumns]
        public class PassengerSchema
        {
            [PrimaryKeyColumn(AutoIncrement = false)]
            [Column("passengerId")]
            public Guid passengerId { get; set; }

            [Column("BookingId")]
            public Guid bookingId { get; set; }

            [Column("firstName")]
            public string firstName { get; set; }

            [Column("lastName")]
            public string lastName { get; set; }

            [Column("birthDate")]
            public DateTime? birthDate { get; set; }

            [Column("gender")]
            public int gender { get; set; }
        }

        [TableName("Trip")]
        [PrimaryKey("tripId", AutoIncrement = false)]
        [ExplicitColumns]
        public class TripSchema
        {
            [PrimaryKeyColumn(AutoIncrement = false)]
            [Column("tripId")]
            public Guid tripId { get; set; }

            [Column("destinationKey")]
            public Guid destinationKey { get; set; }

            [Column("rocketKey")]
            public Guid rocketKey { get; set; }

            [Column("departureUtc")]
            public DateTime? departureUtc { get; set; }

            [Column("arrivalUtc")]
            public DateTime? arrivalUtc { get; set; }

            [Column("passengerCount")]
            public int passengerCount { get; set; }

            [Column("price")]
            public double price { get; set; }

            [Column("tripStatus")]
            public int tripStatus { get; set; }
        }
    }
}