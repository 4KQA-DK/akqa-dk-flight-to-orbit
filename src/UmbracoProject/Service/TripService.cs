using UmbracoProject.Models;
using UmbracoProject.Repository;
using UmbracoProject.DTO;
using Umbraco.Cms.Core.Services;
using UmbracoProject.Pagination; 

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

        public async Task<TripFilterResponse> GetFilteredTripsAsync(TripFilterRequest filter, PageParameters page)
        {

            try
            {
                var pages = await _tripRepository.GetFilteredTripsAsync(filter, page);

                if (filter.DepartureDate.HasValue)
                {
                    if (pages.Items.Count > 0)
                    {
                        return new TripFilterResponse
                        {
                            ExactMatches = EnrichTrips(pages.Items),
                            NearbyTrips = new(),
                            SearchedDate = filter.DepartureDate,
                            HasExactMatches = true,

                            TotalTrips = pages.TotalCount,
                            PageNumber = pages.Page,
                            PageSize = pages.PageSize,
                            HasNextPage = pages.HasNextPage,
                            HasPreviousPage = pages.HasPreviousPage
                        };
                    }

                    var nextTrips = await _tripRepository.FindNearbyTripsAsync(filter);

                    return new TripFilterResponse
                    {
                        ExactMatches = new(),
                        NearbyTrips = EnrichTrips(nextTrips),
                        SearchedDate = filter.DepartureDate,
                        HasExactMatches = false,
                        Message = nextTrips.Count > 0
                            ? "No trips are available on the selected date. Showing the nearest available departures that match your current filters."
                            : "No trips match your current filters. Try another date or destination.",

                        TotalTrips = pages.TotalCount,
                        PageNumber = pages.Page,
                        PageSize = pages.PageSize,
                        HasNextPage = false,
                        HasPreviousPage = pages.HasPreviousPage && pages.TotalCount > 0
                    };
                }

                return new TripFilterResponse
                {
                    ExactMatches = EnrichTrips(pages.Items),
                    NearbyTrips = new(),
                    SearchedDate = null,
                    HasExactMatches = pages.Items.Count > 0,
                    Message = pages.Items.Count == 0 ? "No trips match your current filters." : null,

                    // paging
                    TotalTrips = pages.TotalCount,
                    PageNumber = pages.Page,
                    PageSize = page.PageSize,
                    HasNextPage = pages.HasNextPage,
                    HasPreviousPage = pages.HasPreviousPage
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
