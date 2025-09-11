// /Service/BookingService.cs
using UmbracoProject.DTO;
using UmbracoProject.Models;
using UmbracoProject.Repository;

namespace UmbracoProject.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IAdminTripService _adminTripService;

        public BookingService(IBookingRepository bookingRepo, IAdminTripService adminTripService)
        {
            _bookingRepo = bookingRepo;
            _adminTripService = adminTripService;
        }

        public async Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var trip = await _adminTripService.GetTripAsync(request.TripId) ?? throw new ArgumentException("Trip not found.", nameof(request.TripId));

            if (trip.TripStatus != TripStatus.Schedueled)
            {
                throw new InvalidOperationException("Cannot book on a trip that is not scheduled.");
            }
                

            var seatsRequested = request.Passengers.Count;
            var seatsLeft = trip.PassengerCount;
            if (seatsLeft < seatsRequested)
            {
                throw new InvalidOperationException($"Not enough seats. Left: {seatsLeft}, Requested: {seatsRequested}.");
            }
                

            var reserved = await _bookingRepo.TryReserveSeatsAsync(trip.TripId, seatsRequested);

            if (!reserved)
            {
                throw new InvalidOperationException("Failed to reserve seats.");
            }
                

            var subtotal = trip.Price * seatsRequested;

            var bookingId = Guid.NewGuid();
            var booking = new Booking
            {
                bookingId = bookingId,
                tripId = trip.TripId,
                price = subtotal,
                date = DateTime.UtcNow
            };

            var passengerRows = request.Passengers.Select(p => new Passenger
            {
                passengerId = Guid.NewGuid(),
                bookingId = bookingId,
                firstName = p.FirstName,
                lastName = p.LastName,
                Email = p.Email,
                birthDate = p.BirthDate.ToDateTime(TimeOnly.MinValue),
                gender = p.Gender
            });

            await _bookingRepo.CreateBookingAsync(booking, passengerRows);

            return new CreateBookingResponse
            {
                BookingId = bookingId,
                TripId = trip.TripId,
                PassengerCount = seatsRequested,
                TotalPrice = subtotal,
                BookedAtUtc = booking.date
            };
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetBookingAsync(bookingId) ?? throw new KeyNotFoundException("Booking not found.");
            
            var trip = await _adminTripService.GetTripAsync(booking.tripId) ?? throw new KeyNotFoundException("Trip not found.");
            
            if (trip.TripStatus != TripStatus.Schedueled && trip.TripStatus != TripStatus.SoldOut)
            {
                throw new InvalidOperationException("Cannot cancel a booking for a trip that is not scheduled.");
            }
            var passengers = await _bookingRepo.GetPassengersForBookingAsync(bookingId);
            var passengerCount = passengers.Count;
            var cancelled = await _bookingRepo.CancelBookingAsync(bookingId);
            if (!cancelled)
            {
                throw new InvalidOperationException("Failed to cancel booking.");
            }
            var released = await _bookingRepo.ReleaseSeatsAsync(trip.TripId, passengerCount);
            if (!released)
            {
                throw new InvalidOperationException("Failed to release seats.");
            }
            return true;
        }


        public async Task<GetBookingResponse> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _bookingRepo.GetBookingAsync(bookingId)
                          ?? throw new KeyNotFoundException("Booking not found.");

            var passengers = await _bookingRepo.GetPassengersForBookingAsync(bookingId);

            return new GetBookingResponse
            {
                BookingId = booking.bookingId,
                TripId = booking.tripId,
                Price = booking.price,
                Date = booking.date,
                Passengers = passengers.Select(p => new GetBookingResponse.PassengerItem
                {
                    PassengerId = p.passengerId,
                    FirstName = p.firstName,
                    LastName = p.lastName,
                    Email = p.Email,
                    BirthDate = DateOnly.FromDateTime(p.birthDate),
                    Gender = p.gender
                }).ToList()
            };
        }

        public async Task<List<GetBookingResponse>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            var results = new List<GetBookingResponse>(bookings.Count);
            foreach (var b in bookings)
            {
                var pax = await _bookingRepo.GetPassengersForBookingAsync(b.bookingId);
                results.Add(new GetBookingResponse
                {
                    BookingId = b.bookingId,
                    TripId = b.tripId,
                    Price = b.price,
                    Date = b.date,
                    Passengers = pax.Select(p => new GetBookingResponse.PassengerItem
                    {
                        PassengerId = p.passengerId,
                        FirstName = p.firstName,
                        LastName = p.lastName,
                        Email = p.Email,
                        BirthDate = DateOnly.FromDateTime(p.birthDate)
                    }).ToList()
                });
            }
            return results;
        }

        public async Task<List<GetBookingResponse>> GetByTripAsync(Guid tripId)
        {

            var trip = await _adminTripService.GetTripAsync(tripId);
            if (trip is null)
            {
                throw new KeyNotFoundException("Trip not found."); 
            }

            var bookings = await _bookingRepo.GetBookingsForTripAsync(tripId);

            var results = new List<GetBookingResponse>(bookings.Count);
            foreach (var b in bookings)
            {
                var pax = await _bookingRepo.GetPassengersForBookingAsync(b.bookingId);
                results.Add(new GetBookingResponse
                {
                    BookingId = b.bookingId,
                    TripId = b.tripId,
                    Price = b.price,
                    Date = b.date,
                    Passengers = pax.Select(p => new GetBookingResponse.PassengerItem
                    {
                        PassengerId = p.passengerId,
                        FirstName = p.firstName,
                        LastName = p.lastName,
                        Email = p.Email,
                        BirthDate = DateOnly.FromDateTime(p.birthDate),
                        Gender = p.gender
                    }).ToList()
                });
            }

            return results;
        }

        public async Task<bool> TryReserveSeatsAsync(Guid id, int seats)
        {
            

            var success = await _bookingRepo.TryReserveSeatsAsync(id, seats);
            if (!success)
            {
                throw new InvalidOperationException("Failed to reserve seats.");

            }
            else
            {
                return success;
            }
        }
    }
}
