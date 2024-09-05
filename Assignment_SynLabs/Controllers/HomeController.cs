using Assignment_SynLabs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment_SynLabs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly RecruitmentDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(RecruitmentDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("secure")]
        public IActionResult SecureEndpoint()
        {
            // This endpoint is secured and requires authentication
            return Ok("This is a secure endpoint. You are authenticated.");
        }



    }
}
