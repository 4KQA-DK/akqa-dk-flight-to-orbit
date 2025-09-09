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

        public async Task<RocketStatus> GetAsync(Guid rocketKey)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;
            var sql = new NPoco.Sql("SELECT * FROM [RocketStatus] WHERE [rocketKey] = @0", rocketKey);
            var row = await db.SingleOrDefaultAsync<RocketStatus>(sql);
            scope.Complete();
            return row;
        }

        public async Task<List<RocketStatus>> GetAllAsync()
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;
            var sql = new NPoco.Sql("SELECT * FROM [RocketStatus]");
            var rows = await db.FetchAsync<RocketStatus>(sql);
            scope.Complete();
            return rows;
        }
        public async Task<RocketStatus> CreateAsync(RocketStatus row)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            await scope.Database.InsertAsync(row);
            return row;
        }

        public async Task UpdateAsync(RocketStatus row)
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.Database.UpdateAsync(row);
            scope.Complete();
        }

        public async Task DeleteAsync(Guid rocketKey)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);
            await scope.Database.ExecuteAsync(
                new NPoco.Sql("DELETE FROM [RocketStatus] WHERE [rocketKey] = @0", rocketKey));
        }

    }
}
