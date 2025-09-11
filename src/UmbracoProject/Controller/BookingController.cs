using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UmbracoProject.DTO;
using UmbracoProject.Service;

namespace UmbracoProject.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Route("create/booking")]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _bookingService.CreateAsync(request);
                
                return Ok(new
                {
                    bookingId = result.BookingId,
                    tripId = result.TripId,
                    passengerCount = result.PassengerCount,
                    totalPrice = result.TotalPrice,
                    bookedAtUtc = result.BookedAtUtc
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // e.g. not enough seats, trip not schedueled, etc.
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("cancel/booking/{bookingId}")]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            try
            {
                await _bookingService.CancelBookingAsync(bookingId);
                return Ok(new { message = "Booking cancelled successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("get/booking/{bookingId}")]
        public async Task<IActionResult> GetBooking(Guid bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("get/all/bookings")]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet]
        [Route("get/bookings/bytrip/{tripId}")]
        public async Task<IActionResult> GetByTrip(Guid tripId)
        {
            try
            {
                var bookings = await _bookingService.GetByTripAsync(tripId);
                return Ok(bookings);
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(new { error = ex.Message });
            }
            
        }
    }
}
