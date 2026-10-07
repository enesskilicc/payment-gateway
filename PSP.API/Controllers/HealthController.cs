using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PSP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        [HttpGet]
        public IActionResult Check()
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK, new
                {
                    status = "Healthy"
                 
                });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    status = "Unhealthy",
                    error = ex.Message
                });
            }
        }
    }
}
