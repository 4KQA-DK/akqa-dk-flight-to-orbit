using UmbracoProject.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace UmbracoProject.Repository
{
    public class TripRepository:ITripRepository
    {
        private readonly IScopeProvider _scopeProvider;

        public TripRepository(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task CreateTripAsync(Trip trip)
        {
            using var scope = _scopeProvider.CreateScope();
            var db = scope.Database;
            await db.InsertAsync(trip);
            scope.Complete();
        }
    }
}
