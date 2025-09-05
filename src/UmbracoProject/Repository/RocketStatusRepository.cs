using UmbracoProject.Models;
using Umbraco.Cms.Infrastructure.Scoping;


namespace UmbracoProject.Repository
{
    public class RocketStatusRepository:IRocketStatusRepository
    {
        private readonly IScopeProvider _scopeProvider;

        public RocketStatusRepository(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task<RocketStatus?> GetAsync(Guid rocketKey)
        {
            if (rocketKey == Guid.Empty) throw new ArgumentException("rocketKey is required.", nameof(rocketKey));

            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;

            var sql = new NPoco.Sql("SELECT * FROM [RocketStatus] WHERE [rocketKey] = @0", rocketKey);
            var rocketstatus = await db.SingleOrDefaultAsync<RocketStatus>(sql);

            scope.Complete();
            return rocketstatus;
        }

        public async Task<RocketStatus> UpsertAsync(Guid rocketKey, RocketStatusCode status)
        {
            if (rocketKey == Guid.Empty) throw new ArgumentException("rocketKey is required.", nameof(rocketKey));

            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            var db = scope.Database;

            var existing = await db.SingleOrDefaultAsync<RocketStatus>(
                new NPoco.Sql("SELECT * FROM [RocketStatus] WHERE [rocketKey] = @0", rocketKey));

            var now = DateTime.UtcNow;

            if (existing is null)
            {
                await db.InsertAsync(new RocketStatus
                {
                    rocketKey = rocketKey,
                    rocketStatus = status,
                    lastUpdatedUtc = now
                });
            }
            else
            {
                if (existing.rocketStatus != status)
                {
                    existing.rocketStatus = status;
                    existing.lastUpdatedUtc = now;
                    await db.UpdateAsync(existing);
                }
            }

            return existing;
        }
    }
}
