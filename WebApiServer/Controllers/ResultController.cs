using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetResult()
        {
            return Ok(new
            {
                name = "pedram",
                family = "asadipour",
                age = 19,
                education = "Diplom"
            });
        }
    }
}
