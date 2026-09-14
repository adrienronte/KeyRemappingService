using Microsoft.AspNetCore.Mvc;

namespace KeyRemappingService.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class KeyRemappingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Le web service fonctionne !");
        }
    }
}
