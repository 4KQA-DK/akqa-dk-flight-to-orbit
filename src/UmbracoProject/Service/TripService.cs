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
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.RocketKey == Guid.Empty)
            {
                throw new ArgumentException("RocketKey is required.");
            }

            if (request.DestinationKey == Guid.Empty) 
            { 
                throw new ArgumentException("DestinationKey is required."); 
            }

            if (request.ArrivalUtc <= request.DepartureUtc)
            {
                throw new ArgumentException("ArrivalUtc must be after DepartureUtc.");
            }


            var rocket = _contentService.GetById(request.RocketKey);
            if (rocket == null)
            {
                throw new ArgumentException("Rocket not found.");
            }
            else if (rocket.Trashed) 
            {
                throw new ArgumentException("Rocket is in recycle bin."); 
            }

            var destination = _contentService.GetById(request.DestinationKey);
            if (destination == null)
            {
                throw new ArgumentException("Destination not found.");

            }
            if (destination.Trashed)
            {
                throw new ArgumentException("Destination is in recycle bin.");
            }
                
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

        public async Task<GetTripResponse> GetTripAsync(Guid tripId)
        {
            var trip = await _tripRepository.GetTripByIdAsync(tripId);
            if (trip is null)
            {
                throw new ArgumentException("Trip not found");
            }
                

            var rocket = _contentService.GetById(trip.rocketKey);
            var destination = _contentService.GetById(trip.destinationKey);

            var dto = new GetTripResponse
            {
                TripId = trip.tripId,
                RocketName = rocket?.Name ?? "(rocket not found)",
                DestinationName = destination?.Name ?? "(destination not found)",
                DepartureUtc = trip.departureUtc,
                ArrivalUtc = trip.arrivalUtc,
                PassengerCount = trip.passengerCount,
                Price = trip.price,
                TripStatus = trip.tripStatus
            };

            return dto;
        }

       
        public async Task<bool> UpdateTripStatusAsync(Guid id, TripStatus newTripStatus)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Trip id is required.", nameof(id));
            }

            var trip = await _tripRepository.GetTripByIdAsync(id);

            if (trip is null)
            {
                throw new KeyNotFoundException("Trip not found.");
            }
                
            if (trip.tripStatus == TripStatus.Schedueled && newTripStatus == TripStatus.Completed)
            {
                throw new InvalidOperationException("Cannot change status from Schedueled directly to Completed. Move to Ongoing first.");

            }

            if (trip.tripStatus == newTripStatus)
            {
                return true;
            }

            var updatedTrip = await _tripRepository.UpdateStatusAsync(id, newTripStatus);
            if (updatedTrip == false)
            {
                throw new InvalidOperationException("Failed to update trip status.");

            }
            else
            {
                return true;
            }
        }

        public async Task<List<GetTripResponse>> GetAllTripsAsync()
        {
            List<Trip> trips = new List<Trip>();
            try
            {
                trips = (await _tripRepository.GetAllTripsAsync()).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load trips from database.", ex);
            }

            var result = new List<GetTripResponse>(trips.Count);

            foreach (var trip in trips)
            {
                string rocketName = "(rocket not found)";
                string destinationName = "(destination not found)";

                try
                {
                    var rocket = _contentService.GetById(trip.rocketKey);
                    if (rocket is not null && !rocket.Trashed)
                        rocketName = rocket.Name;

                    var destination = _contentService.GetById(trip.destinationKey);
                    if (destination is not null && !destination.Trashed)
                        destinationName = destination.Name;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to resolve rocket/destination names.", ex);
                }

                result.Add(new GetTripResponse
                {
                    TripId = trip.tripId,
                    RocketName = rocketName,
                    DestinationName = destinationName,
                    DepartureUtc = trip.departureUtc,
                    ArrivalUtc = trip.arrivalUtc,
                    PassengerCount = trip.passengerCount,
                    Price = trip.price,
                    TripStatus = trip.tripStatus
                });
            }

            return result;
        }

        public async Task<List<GetTripResponse>> GetAllTripsPriceAscAsync()
        {
            List<Trip> trips = new List<Trip>();
            try
            {
                trips = (await _tripRepository.GetAllTripsPriceAscAsync()).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load trips from database.", ex);
            }

            var result = new List<GetTripResponse>(trips.Count);

            foreach (var trip in trips)
            {
                string rocketName = "(rocket not found)";
                string destinationName = "(destination not found)";

                try
                {
                    var rocket = _contentService.GetById(trip.rocketKey);
                    if (rocket is not null && !rocket.Trashed)
                        rocketName = rocket.Name;

                    var destination = _contentService.GetById(trip.destinationKey);
                    if (destination is not null && !destination.Trashed)
                        destinationName = destination.Name;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to resolve rocket/destination names.", ex);
                }

                result.Add(new GetTripResponse
                {
                    TripId = trip.tripId,
                    RocketName = rocketName,
                    DestinationName = destinationName,
                    DepartureUtc = trip.departureUtc,
                    ArrivalUtc = trip.arrivalUtc,
                    PassengerCount = trip.passengerCount,
                    Price = trip.price,
                    TripStatus = trip.tripStatus
                });
            }

            return result;

        }

        public async Task<List<GetTripResponse>> GetAllTripsTravelTimeAscAsync()
        {
            List<Trip> trips = new List<Trip>();
            try
            {
                trips = (await _tripRepository.GetAllTripsTravelTimeAscAsync()).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load trips from database.", ex);
            }

            var result = new List<GetTripResponse>(trips.Count);

            foreach (var trip in trips)
            {
                string rocketName = "(rocket not found)";
                string destinationName = "(destination not found)";

                try
                {
                    var rocket = _contentService.GetById(trip.rocketKey);
                    if (rocket is not null && !rocket.Trashed)
                        rocketName = rocket.Name;

                    var destination = _contentService.GetById(trip.destinationKey);
                    if (destination is not null && !destination.Trashed)
                        destinationName = destination.Name;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to resolve rocket/destination names.", ex);
                }

                result.Add(new GetTripResponse
                {
                    TripId = trip.tripId,
                    RocketName = rocketName,
                    DestinationName = destinationName,
                    DepartureUtc = trip.departureUtc,
                    ArrivalUtc = trip.arrivalUtc,
                    PassengerCount = trip.passengerCount,
                    Price = trip.price,
                    TripStatus = trip.tripStatus
                });
            }

            return result;
        }

        public async Task<List<GetTripResponse>> GetAllTripsByDestinationAsync(Guid destinationId)
        {
            if (destinationId == Guid.Empty)
            {
                throw new ArgumentException("Destination id is required.", nameof(destinationId));
            }
            var destination = _contentService.GetById(destinationId);
            if (destination == null)
            {
                throw new ArgumentException("Destination not found.");
            }
            if (destination.Trashed)
            {
                throw new ArgumentException("Destination is in recycle bin.");
            }
            List<Trip> trips = new List<Trip>();
            try
            {
                trips = (await _tripRepository.GetAllTripsByDestination(destinationId)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load trips from database.", ex);
            }
            var result = new List<GetTripResponse>(trips.Count);
            foreach (var trip in trips)
            {
                string rocketName = "(rocket not found)";
                string destinationName = "(destination not found)";
                try
                {
                    var rocket = _contentService.GetById(trip.rocketKey);
                    if (rocket is not null && !rocket.Trashed)
                        rocketName = rocket.Name;
                    var dest = _contentService.GetById(trip.destinationKey);
                    if (dest is not null && !dest.Trashed)
                        destinationName = dest.Name;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to resolve rocket/destination names.", ex);
                }
                result.Add(new GetTripResponse
                {
                    TripId = trip.tripId,
                    RocketName = rocketName,
                    DestinationName = destinationName,
                    DepartureUtc = trip.departureUtc,
                    ArrivalUtc = trip.arrivalUtc,
                    PassengerCount = trip.passengerCount,
                    Price = trip.price,
                    TripStatus = trip.tripStatus
                });
            }
            return result;
        }

        public async Task<List<GetTripResponse>> GetAvailableTripsAsync(int groupSize)
        {
            if (groupSize <= 0) throw new ArgumentException("Group size must be > 0.", nameof(groupSize));

            var trips = await _tripRepository.GetScheduledTripsWithMinCapacityAsync(groupSize);

            var result = new List<GetTripResponse>(trips.Count);
            foreach (var t in trips)
            {
                var rocket = _contentService.GetById(t.rocketKey);
                var dest = _contentService.GetById(t.destinationKey);

                result.Add(new GetTripResponse
                {
                    TripId = t.tripId,
                    RocketName = rocket?.Name ?? "(rocket not found)",
                    DestinationName = dest?.Name ?? "(destination not found)",
                    DepartureUtc = t.departureUtc,
                    ArrivalUtc = t.arrivalUtc,
                    PassengerCount = t.passengerCount,
                    Price = t.price,
                    TripStatus = t.tripStatus
                });
            }
            return result;
        }

        public async Task<TripFilterResponse> GetFilteredTripsAsync(TripFilterRequest filter)
        {
            try
            {
                // 1) Hent præcise ture (respekterer destination + passagerCount hvis sat)
                var exactTrips = await _tripRepository.GetFilteredTripsAsync(filter);

                if (filter.DepartureDate.HasValue)
                {
                    if (exactTrips.Count > 0)
                    {
                        // → Der findes ture på datoen: vis KUN dem
                        return new TripFilterResponse
                        {
                            ExactMatches = EnrichTripsAsync(exactTrips),
                            NearbyTrips = new(),
                            SearchedDate = filter.DepartureDate,
                            HasExactMatches = true
                        };
                    }

                    // → Ingen ture på datoen: vis de 5 NÆSTE efter datoen (stadig filtreret)
                    var nextTrips = await _tripRepository.FindNearbyTripsAsync(filter);
                    return new TripFilterResponse
                    {
                        ExactMatches = new(),
                        NearbyTrips = EnrichTripsAsync(nextTrips),
                        SearchedDate = filter.DepartureDate,
                        HasExactMatches = false,
                        Message = $"$\"\"No trips are available on the selected date. Showing the nearest available departures that match your current filters.\"\"\r\n"
                    };
                }

                // 2) Ingen dato valgt → vis alle der matcher destination + passagerer
                return new TripFilterResponse
                {
                    ExactMatches = EnrichTripsAsync(exactTrips),
                    NearbyTrips = new(),
                    SearchedDate = null,
                    HasExactMatches = exactTrips.Count > 0
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error occurred while filtering trips.", ex);
            }
        }


        /// <summary>
        /// Enriches trip data with rocket and destination information.
        /// </summary>
        /// <param name="trips">Raw trip entities to enrich</param>
        /// <returns>List of enriched trip responses</returns>
        private List<GetTripResponse> EnrichTripsAsync(List<Trip> trips)
        {
            var result = new List<GetTripResponse>(trips.Count);

            foreach (var trip in trips)
            {
                // Get rocket and destination details
                var rocket = _contentService.GetById(trip.rocketKey);
                var destination = _contentService.GetById(trip.destinationKey);

                result.Add(new GetTripResponse
                {
                    TripId = trip.tripId,
                    RocketName = rocket?.Name ?? "(rocket not found)",
                    DestinationName = destination?.Name ?? "(destination not found)",
                    DepartureUtc = trip.departureUtc,
                    ArrivalUtc = trip.arrivalUtc,
                    PassengerCount = trip.passengerCount,
                    Price = trip.price,
                    TripStatus = trip.tripStatus
                });
            }

            return result;
        }
    }

}
