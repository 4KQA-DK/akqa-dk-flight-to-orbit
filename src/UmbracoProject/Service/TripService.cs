using UmbracoProject.Models;
using UmbracoProject.Repository;
using UmbracoProject.DTO;
using Umbraco.Cms.Core.Services;   

namespace UmbracoProject.Service
{
    public class TripService: ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IContentService _contentService;

        public TripService(ITripRepository tripRepository, IContentService contentService)
        {
            _tripRepository = tripRepository;
            _contentService = contentService;

        }

        public async Task<Guid> CreateTripAsync(CreateTripRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (request.RocketKey == Guid.Empty) throw new ArgumentException("RocketKey is required.");
            if (request.DestinationKey == Guid.Empty) throw new ArgumentException("DestinationKey is required.");
            if (request.ArrivalUtc <= request.DepartureUtc)
                throw new ArgumentException("ArrivalUtc must be after DepartureUtc.");

            // (valgfrit) valider Umbraco-indholdet
            var rocket = _contentService.GetById(request.RocketKey)
                         ?? throw new ArgumentException("Rocket not found.");
            if (rocket.Trashed) throw new ArgumentException("Rocket is in recycle bin.");

            var destination = _contentService.GetById(request.DestinationKey)
                             ?? throw new ArgumentException("Destination not found.");
            if (destination.Trashed) throw new ArgumentException("Destination is in recycle bin.");

            var trip = new Trip
            {
                tripId = Guid.NewGuid(),
                rocketKey = request.RocketKey,
                destinationKey = request.DestinationKey,
                departureUtc = request.DepartureUtc,
                arrivalUtc = request.ArrivalUtc,
                passengerCount = request.PassengerCount,
                price = request.Price,
                tripStatus = TripStatus.Schedueled
            };

            await _tripRepository.CreateTripAsync(trip);
            return trip.tripId;
        }
    }

}
