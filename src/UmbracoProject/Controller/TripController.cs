using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UmbracoProject.Models;
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

    }
}
