using APP.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Books.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbController : ControllerBase
    {
        [HttpGet, Route("~/api/SeedDb")] // route for the get action
        // changed with the Route
        // attribute: api/SeedDb
        public IActionResult Seed()
        {
            // Initialize the UsersDbFactory instance.
            var dbFactory = new DbFactory();
            // Seed the database using the SeedDb method.
            dbFactory.SeedDb();
            // Return HTTP OK status result (200) with a success message.
            return Ok("Database seed successful.");
        }

    }
}