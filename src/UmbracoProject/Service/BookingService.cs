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

            var trip = await _adminTripService.GetTripAsync(request.TripId)
                       ?? throw new ArgumentException("Trip not found.", nameof(request.TripId));

            if (trip.TripStatus != TripStatus.Schedueled)
            {
                throw new InvalidOperationException("Cannot book on a trip that is not scheduled.");
            }
                

            var bookedSeats = await _bookingRepo.GetBookedSeatsAsync(trip.TripId);
            var seatsLeft = trip.PassengerCount - bookedSeats;
            if (seatsLeft < request.Passengers.Count)
            {
                throw new InvalidOperationException($"Not enough seats. Left: {seatsLeft}, Requested: {request.Passengers.Count}.");
            }

            var subtotal = trip.Price * request.Passengers.Count;

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
            await TryReserveSeatsAsync(request.TripId, request.Passengers.Count());

            return new CreateBookingResponse
            {
                BookingId = bookingId,
                TripId = trip.TripId,
                PassengerCount = request.Passengers.Count,
                TotalPrice = subtotal,
                BookedAtUtc = booking.date
            };
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
