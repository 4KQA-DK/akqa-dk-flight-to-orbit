using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using UmbracoProject.DTO;
using UmbracoProject.Models;
using UmbracoProject.Repository;

namespace UmbracoProject.Service
{
    public class AdminTripService : IAdminTripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IRocketStatusService _rocketStatusService;
        private readonly IContentService _contentService;
        private readonly IPriceCalculatorService _priceCalculatorService;
        private readonly ITravelTimeEstimator _travelTimeEstimator;

        public AdminTripService(
            ITripRepository tripRepository,
            IRocketStatusService rocketStatusService,
            IContentService contentService,
            IPriceCalculatorService priceCalculatorService,
            ITravelTimeEstimator travelTimeEstimator)
        {
            _tripRepository = tripRepository;
            _rocketStatusService = rocketStatusService;
            _contentService = contentService;
            _priceCalculatorService = priceCalculatorService;
            _travelTimeEstimator = travelTimeEstimator;
        }

        public async Task<Guid> CreateTripAsync(CreateTripRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateRocketAndDestination(request.RocketKey, request.DestinationKey);

            var status = await _rocketStatusService.TryGetAsync(request.RocketKey)!;
            if (status.rocketStatus is RocketStatusCode.Maintenance)
            {
                throw new InvalidOperationException("Rocket is not available for scheduling.");
            }

            var price = _priceCalculatorService.CalculatePrice(
                request.RocketKey,
                request.DestinationKey,
                request.DiscountPercent);

            var duration = _travelTimeEstimator.EstimateTravelTime(
                request.RocketKey,
                request.DestinationKey);

            var arrival = request.DepartureUtc + duration;

            await CheckForOverlapsAsync(request.RocketKey, request.DepartureUtc, arrival, 1);

            var trip = new Trip
            {
                tripId = Guid.NewGuid(),
                rocketKey = request.RocketKey,
                destinationKey = request.DestinationKey,
                departureUtc = request.DepartureUtc,
                arrivalUtc = arrival,
                passengerCount = request.AvalivableSeats,
                price = price,
                tripStatus = TripStatus.Schedueled
            };

            var capacity = GetRocketCapacity(request.RocketKey);
            if (request.AvalivableSeats > capacity)
            {
                throw new InvalidOperationException(
                    $"Number of seats: ({request.AvalivableSeats}) exceeds rocket capacity ({capacity}).");
            }

            await _tripRepository.CreateTripAsync(trip);
            await _rocketStatusService.UpdateAsync(request.RocketKey, RocketStatusCode.Reserved);

            return trip.tripId;
        }

        public async Task<List<GetTripResponse>> GetAllTripsAsync()
        {
            var trips = (await _tripRepository.GetAllTripsAsync()).ToList();
            var result = new List<GetTripResponse>(trips.Count);

            foreach (var trip in trips)
            {
                string rocketName = "(rocket not found)";
                string destinationName = "(destination not found)";

                var rocket = _contentService.GetById(trip.rocketKey);
                if (rocket is not null && !rocket.Trashed)
                {
                    rocketName = rocket.Name;
                }

                var destination = _contentService.GetById(trip.destinationKey);
                if (destination is not null && !destination.Trashed)
                {
                    destinationName = destination.Name;
                }

                var duration = trip.arrivalUtc - trip.departureUtc;
                var estDays = (int)Math.Round(duration.TotalDays, MidpointRounding.AwayFromZero);

                result.Add(new GetTripResponse
                {
                    TripId = trip.tripId,
                    RocketName = rocketName,
                    DestinationName = destinationName,
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

        public async Task<GetTripResponse> GetTripAsync(Guid tripId)
        {
            var trip = await _tripRepository.GetTripByIdAsync(tripId);
            if (trip == null)
            {
                throw new KeyNotFoundException($"Trip with ID {tripId} not found");
            }

            var rocket = _contentService.GetById(trip.rocketKey);
            var destination = _contentService.GetById(trip.destinationKey);

            var duration = trip.arrivalUtc - trip.departureUtc;
            var estDays = (int)Math.Round(duration.TotalDays, MidpointRounding.AwayFromZero);

            var dto = new GetTripResponse
            {
                TripId = trip.tripId,
                RocketName = rocket?.Name ?? "(rocket not found)",
                DestinationName = destination?.Name ?? "(destination not found)",
                DepartureUtc = trip.departureUtc,
                ArrivalUtc = trip.arrivalUtc,
                EstimatedTravelDays = estDays,
                PassengerCount = trip.passengerCount,
                Price = trip.price,
                TripStatus = trip.tripStatus
            };

            return dto;
        }

        public async Task<bool> UpdateTripStatusAsync(Guid id, TripStatus newTripStatus)
        {
            var trip = await _tripRepository.GetTripByIdAsync(id);
            if (trip is null)
            {
                throw new KeyNotFoundException("Trip not found.");
            }

            if (trip.tripStatus == TripStatus.Schedueled && newTripStatus == TripStatus.Completed)
            {
                throw new InvalidOperationException("Cannot change status from Schedueled directly to Completed. Move to Ongoing first.");
            }

            if ((trip.tripStatus == TripStatus.Ongoing || trip.tripStatus == TripStatus.Completed)
                && (newTripStatus == TripStatus.Cancelled || newTripStatus == TripStatus.Schedueled))
            {
                throw new InvalidOperationException("Cannot change status from Ongoing or Completed to Cancelled or Scheduled.");
            }

            if (trip.tripStatus == newTripStatus)
            {
                return true;
            }

            var success = await _tripRepository.UpdateStatusAsync(id, newTripStatus);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update trip status.");
            }
            else
            {
                return true;
            }
        }

        public async Task CancelTripAsync(Guid tripId)
        {
            if (tripId == Guid.Empty)
            {
                throw new ArgumentException("Trip id is required.", nameof(tripId));
            }

            var trip = await _tripRepository.GetTripByIdAsync(tripId);
            if (trip == null)
            {
                throw new ArgumentException("Trip not found.", nameof(tripId));
            }

            if (trip.tripStatus == TripStatus.Ongoing)
            {
                throw new InvalidOperationException("Cannot cancel an ongoing trip.");
            }

            if (trip.tripStatus == TripStatus.Completed)
            {
                throw new InvalidOperationException("Cannot cancel a completed trip.");
            }

            if (trip.tripStatus == TripStatus.Cancelled)
            {
                return;
            }

            var ok = await _tripRepository.UpdateStatusAsync(tripId, TripStatus.Cancelled);
            if (!ok)
            {
                throw new InvalidOperationException("Failed to cancel trip.");
            }

            await _rocketStatusService.UpdateAsync(trip.rocketKey, RocketStatusCode.Idle);
        }

        private void ValidateRocketAndDestination(Guid rocketKey, Guid destinationKey)
        {
            var rocket = _contentService.GetById(rocketKey);
            if (rocket == null)
            {
                throw new ArgumentException("Rocket not found.");
            }
            if (rocket.Trashed)
            {
                throw new ArgumentException("Rocket is in recycle bin.");
            }

            var destination = _contentService.GetById(destinationKey);
            if (destination == null)
            {
                throw new ArgumentException("Destination not found.");
            }
            if (destination.Trashed)
            {
                throw new ArgumentException("Destination is in recycle bin.");
            }
        }

        private async Task CheckForOverlapsAsync(Guid rocketKey, DateTime departure, DateTime arrival, int turnaroundDays)
        {
            var startD = DateOnly.FromDateTime(departure);
            var endD = DateOnly.FromDateTime(arrival);

            var overlaps = await _tripRepository.HasOverlappingTripByDateAsync(
                rocketKey, startD, endD, turnaroundDays);

            if (overlaps)
            {
                throw new InvalidOperationException("Rocket is already booked in that date range.");
            }
        }

        private IContent GetRocketModel(Guid rocketKey)
        {
            var rocket = _contentService.GetById(rocketKey);
            if (rocket == null)
            {
                throw new KeyNotFoundException($"Rocket {rocketKey} not found");
            }

            var udiString = rocket.GetValue<string>("model");
            if (string.IsNullOrWhiteSpace(udiString))
            {
                throw new InvalidOperationException($"Rocket {rocketKey} has no model assigned");
            }

            var modelGuidPart = udiString.Split('/').Last();
            var modelId = Guid.Parse(modelGuidPart);

            var model = _contentService.GetById(modelId);
            if (model == null)
            {
                throw new KeyNotFoundException($"Rocket model {modelId} not found");
            }

            return model;
        }

        private int GetRocketCapacity(Guid rocketKey)
        {
            var model = GetRocketModel(rocketKey);
            return model.GetValue<int>("capacity");
        }
    }
}
