using KeyRemappingService.Data;
using Microsoft.AspNetCore.Mvc;

namespace KeyRemappingService.Controllers
{
    [ApiController]
    [Route("api/keyboards")]
    public class KeyboardsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public KeyboardsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetKeyboards()
        {
            var keyboards = _db.Keyboards
                .Select(k => new
                {
                    id = k.Id,
                    name = k.Name
                })
                .ToList();

            return Ok(keyboards);
        }
    }
}