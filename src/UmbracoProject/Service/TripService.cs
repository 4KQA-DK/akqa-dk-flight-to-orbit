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
        private readonly IRocketStatusService _rocketStatusService;

        public TripService(ITripRepository tripRepository, IContentService contentService, IRocketStatusService rocketStatusService)
        {
            _tripRepository = tripRepository;
            _contentService = contentService;
            _rocketStatusService = rocketStatusService;
        }

        public async Task<TripFilterResponse> GetFilteredTripsAsync(TripFilterRequest filter, int pageNumber)
        {
            try
            {
                var exactTrips = await _tripRepository.GetFilteredTripsAsync(filter, pageNumber);

                if (filter.DepartureDate.HasValue)
                {
                    if (exactTrips.Count > 0)
                    {
                        return new TripFilterResponse
                        {
                            ExactMatches = EnrichTrips(exactTrips),
                            NearbyTrips = new(),
                            SearchedDate = filter.DepartureDate,
                            HasExactMatches = true
                        };
                    }

                    var nextTrips = await _tripRepository.FindNearbyTripsAsync(filter);

                    if (nextTrips.Count > 0)
                    {
                        return new TripFilterResponse
                        {
                            ExactMatches = new(),
                            NearbyTrips = EnrichTrips(nextTrips),
                            SearchedDate = filter.DepartureDate,
                            HasExactMatches = false,
                            Message = "No trips are available on the selected date. Showing the nearest available departures that match your current filters."
                        };
                    }

                    return new TripFilterResponse
                    {
                        ExactMatches = new(),
                        NearbyTrips = new(),
                        SearchedDate = filter.DepartureDate,
                        HasExactMatches = false,
                        Message = "No trips match your current filters. Try another date or destination."
                    };
                }

                return new TripFilterResponse
                {
                    ExactMatches = EnrichTrips(exactTrips),
                    NearbyTrips = new(),
                    SearchedDate = null,
                    HasExactMatches = exactTrips.Count > 0,
                    Message = exactTrips.Count == 0 ? ("No trips match your current filters.") : null
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error occurred while filtering trips.", ex);
            }
        }


        private List<GetTripResponse> EnrichTrips(List<Trip> trips)
        {
            var result = new List<GetTripResponse>(trips.Count);

            foreach (var trip in trips)
            {
                var rocket = _contentService.GetById(trip.rocketKey);
                var destination = _contentService.GetById(trip.destinationKey);
                var duration = trip.arrivalUtc - trip.departureUtc;
                var estDays = (int)Math.Round(duration.TotalDays, MidpointRounding.AwayFromZero);

                result.Add(new GetTripResponse
                {
                    TripId = trip.tripId,
                    RocketName = rocket?.Name ?? "(rocket not found)",
                    RocketKey = trip.rocketKey,
                    DestinationName = destination?.Name ?? "(destination not found)",
                    DestinationKey = trip.destinationKey,
                    DepartureUtc = trip.departureUtc,
                    ArrivalUtc = trip.arrivalUtc,
                    EstimatedTravelDays = estDays,
                    PassengerCount = trip.passengerCount,
                    Price = trip.price,
                    TripStatus = trip.tripStatus
                });
            }

            return result;
        }

    }

}
