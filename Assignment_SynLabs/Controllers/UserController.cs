using Assignment_SynLabs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]

public class UserController : ControllerBase
{
    private readonly RecruitmentDbContext _context;
    private readonly IConfiguration _configuration;

    public UserController(RecruitmentDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // POST: api/User/signup
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] User newUser)
    {
        // Check if the user already exists
        var userExists = await _context.Users.AnyAsync(u => u.Email == newUser.Email);
        if (userExists)
        {
            return BadRequest(new { message = "User with this email already exists." });
        }

        // Hash the password
        newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);

        // Create Profile for Applicant users
        if (newUser.UserType == UserType.Applicant)
        {
            newUser.Profile = new Profile
            {
                Applicant = newUser,
                Name = newUser.Name,
                Email = newUser.Email,
                Phone = "",
                Education=newUser.Profile.Education,
                Experience=newUser.Profile.Experience,
                ResumeFileAddress=newUser.Profile.ResumeFileAddress,
                Skills=newUser.Profile.Skills

            };
        }

        // Add user to the database
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully!" });
    }

    // POST: api/User/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Generate JWT Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                 
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.UserType.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"], // Ensure the audience is set
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });

    }

    // GET: api/User/profile
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var email = User.Identity.Name; // The user's email from the JWT claims
        var user = await _context.Users.Include(u => u.Profile).FirstAsync();
                                       

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            user.Name,
            user.Email,
            user.Address,
            user.ProfileHeadline,
            user.Profile.Skills,
            user.Profile.Education,
            user.Profile.Experience
        });
    }
}
