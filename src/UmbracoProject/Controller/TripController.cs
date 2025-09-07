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
        private readonly IAdminTripService _adminTripService;

        public TripController(ITripService tripService, IAdminTripService adminTripService)
        {
            _tripService = tripService;
            _adminTripService = adminTripService;
        }

        [HttpPost]
        [Route("create/trip")]
        public async Task<IActionResult> Create([FromBody] CreateTripRequest request)
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
                var id = await _adminTripService.CreateTripAsync(request);
                return Ok(new { tripId = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("get/trip/{id}")]
        public async Task<IActionResult> GetTrip(Guid id)
        {
            try
            {
                var trip = await _adminTripService.GetTripAsync(id);
                return Ok(trip);
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(new { error = ex.Message }); 
            }
        }

        [HttpGet]
        [Route("get/all/trips")]
        public async Task<IActionResult> GetAllTrips()
        {
            var trips = await _adminTripService.GetAllTripsAsync();
            return Ok(trips);
        }

        [HttpPatch]
        [Route("update/tripstatus/{id}/{status}")]
        public async Task<IActionResult> UpdateStatus(Guid id, TripStatus status)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { error = "Valid trip ID is required." });
            }

            if (!Enum.IsDefined(typeof(TripStatus), status))
            {
                return BadRequest(new { error = "Invalid trip status." });
            }

            try
            {
                var result = await _adminTripService.UpdateTripStatusAsync(id, status);
                return Ok(new { success = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }


        [HttpPost("cancel/trip/{tripid}")]
        public async Task<IActionResult> CancelTrip(Guid tripid)
        {
            try
            {
                await _adminTripService.CancelTripAsync(tripid);
                return Ok();
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { error = ex.Message });
            }

        }


        [HttpGet]
        [Route("get/trips/filtered")]
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

        

    }
}
