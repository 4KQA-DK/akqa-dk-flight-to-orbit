using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UmbracoProject.DTO;
using UmbracoProject.Models;
using UmbracoProject.Service;

namespace UmbracoProject.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateTripRequest request)
        {
            try
            {
                var id = await _tripService.CreateTripAsync(request);
                return Ok(new { tripId = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Failed to create trip." });
            }
        }

        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetTrip(Guid id)
        {
            try
            {
                var trip = await _tripService.GetTripAsync(id);
                return Ok(trip);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message });

            }
        }

        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetAllTrips()
        {
            try
            {
                var trip = await _tripService.GetAllTripsAsync();
                return Ok(trip);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message });

            }
        }

        [HttpGet]
        [Route("getallbypriceasc")]
        public async Task<IActionResult> GetAllTripsPriceAsc()
        {
            try
            {
                var trip = await _tripService.GetAllTripsPriceAscAsync();
                return Ok(trip);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message });

            }
        }

        [HttpGet]
        [Route("getallbytimetravelasc")]
        public async Task<IActionResult> GetAllTripsTravelTimeAsc()
        {
            try
            {
                var trip = await _tripService.GetAllTripsTravelTimeAscAsync();
                return Ok(trip);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message });

            }
        }

        [HttpGet]
        [Route("getallbydestination/{destination}")]
        public async Task<IActionResult> GetAllTripsByDestination(Guid destination)
        {
            try
            {
                var trip = await _tripService.GetAllTripsByDestinationAsync(destination);
                return Ok(trip);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Filters space trips based on specified criteria and returns matching scheduled trips.
        /// </summary>
        /// <param name="filter">Filter criteria including departure/arrival dates, destination, and passenger count</param>
        /// <returns>HTTP response containing filtered trip data or error message</returns>
        /// <response code="200">Returns list of filtered trips with rocket and destination details</response>
        /// <response code="500">Internal server error occurred during filtering</response>

        [HttpGet]
        [Route("gettripsfiltered")]
        public async Task<IActionResult> GetFilteredTrips([FromQuery] TripFilterRequest filter)
        {
            try
            {
                var result = await _tripService.GetFilteredTripsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPatch]
        [Route("update/{id}/{status}")]
        public async Task<IActionResult> UpdateStatus(Guid id, TripStatus status)
        {
            var result = await _tripService.UpdateTripStatusAsync(id, status);

            return Ok(result);
        }

        [HttpGet("available/{groupSize}")]
        public async Task<IActionResult> GetAvailableTickets(int groupSize)
        {
            try { return Ok(await _tripService.GetAvailableTripsAsync(groupSize)); }
            catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
            catch { return StatusCode(500, new { error = "Failed to load available trips." }); }
        }

    }
}
