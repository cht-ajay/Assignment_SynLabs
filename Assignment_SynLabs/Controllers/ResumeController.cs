using Assignment_SynLabs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Assignment_SynLabs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Applicant")]
    /*[Authorize]*/ // Only authenticated users can access this endpoint
    public class ResumeController : ControllerBase
    {
        private readonly RecruitmentDbContext _context;

        public ResumeController(RecruitmentDbContext context)
        {
            _context = context;
        }


        [HttpPost("uploadResume")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (file == null || (file.ContentType != "application/pdf" && file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
            {
                return BadRequest("Invalid file type. Only PDF and DOCX files are allowed.");
            }

            var userEmail = User.Identity?.Name;

            // Check if userEmail is null
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("User email is missing.");
            }

            // Proceed with finding the user
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            // Ensure that the user is found
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Ensure the 'Uploads' directory exists
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            // Save the resume
            var filePath = Path.Combine(uploadsDirectory, $"{user.Id}_{file.FileName}");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update user's resume file path
            if (user.Profile == null)
            {
                user.Profile = new Profile();
            }
            user.Profile.ResumeFileAddress = filePath;
            await _context.SaveChangesAsync();

            return Ok("Resume uploaded successfully.");
        }





    }
}
