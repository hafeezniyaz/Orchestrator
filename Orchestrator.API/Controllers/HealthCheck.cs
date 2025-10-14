using Microsoft.AspNetCore.Mvc;

namespace Orchestrator.API.Controllers
{
    [ApiController]
    [Route("api/v1/test")]
    public class HealthCheck: ControllerBase
    {
        public HealthCheck()
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> TestApi()
        {
            return Ok("Orchestrator API server is up and running");
        }

    }
}
