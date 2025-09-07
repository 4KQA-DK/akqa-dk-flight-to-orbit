using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UmbracoProject.Service;
using UmbracoProject.Models;
using UmbracoProject.DTO ;
using System.Threading.Tasks;

namespace UmbracoProject.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RocketStatusController : ControllerBase
    {

        private readonly IRocketStatusService _rocketStatusService;

        public RocketStatusController(IRocketStatusService rocketStatusService)
        {
            _rocketStatusService = rocketStatusService;
        }

        [HttpGet]
        [Route("getrocketstatus/{rocketkey}")]
        public async Task<IActionResult> GetRocketStatus(Guid rocketkey)
        {
            try
            {
                await _rocketStatusService.TryGetAsync(rocketkey);
                return Ok();
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message }); ;
            }
        }

        [HttpPut]
        [Route("update/{rocketkey}/{newstatus}")]
        public async Task<IActionResult> UpdateRocketStatus(Guid rocketkey, RocketStatusCode newstatus)
        {
            try
            {
                await _rocketStatusService.UpdateAsync(rocketkey, newstatus);
                return Ok($"{rocketkey} updated to {newstatus}");
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { error = ex.Message }); ;
            }

        }
    }
}
