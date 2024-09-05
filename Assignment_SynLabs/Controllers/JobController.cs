using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment_SynLabs.Models; // Adjust according to your namespace

namespace Assignment_SynLabs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly RecruitmentDbContext _context;

        public JobController(RecruitmentDbContext context)
        {
            _context = context;
        }

        // POST: api/Job/admin/job
        [HttpPost("admin/job")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateJob([FromBody] Job job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the ID of the current admin user
            var adminEmail = User.Identity.Name;
            var admin = await _context.Users.SingleOrDefaultAsync(u => u.Email == adminEmail);

            if (admin == null || admin.UserType != UserType.Admin)
            {
                return Unauthorized();
            }

            // Set the PostedBy property
            job.PostedBy = admin;

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJob), new { job_id = job.Id }, job);
        }

        // GET: api/Job/admin/job/{job_id}
        [HttpGet("admin/job/{job_id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetJob(int job_id)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == job_id);

            if (job == null)
            {
                return NotFound();
            }

            // Retrieve applicants for the job
            var applicants = await _context.Applications
                .Include(a => a.Applicant)
                .Where(a => a.JobId == job_id)
                .Select(a => a.Applicant)
                .ToListAsync();

            var jobDetails = new
            {
                Job = job,
                Applicants = applicants
            };

            return Ok(jobDetails);
        }

        // GET: api/Job/jobs
        [HttpGet("jobs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobs()
        {
            var jobs = await _context.Jobs.ToListAsync();
            return Ok(jobs);
        }

        // POST: api/Job/jobs/apply?job_id={job_id}
        [HttpPost("jobs/apply")]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> ApplyForJob([FromQuery] int job_id)
        {
            var job = await _context.Jobs.FindAsync(job_id);
            if (job == null)
            {
                return NotFound();
            }

            var applicantEmail = User.Identity.Name;
            var applicant = await _context.Users.SingleOrDefaultAsync(u => u.Email == applicantEmail);

            if (applicant == null || applicant.UserType != UserType.Applicant)
            {
                return Unauthorized();
            }

            var existingApplication = await _context.Applications
                .AnyAsync(a => a.JobId == job_id && a.ApplicantId == applicant.Id);

            if (existingApplication)
            {
                return Conflict("You have already applied for this job.");
            }

            var application = new Application
            {
                JobId = job_id,
                ApplicantId = applicant.Id
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok("Applied successfully.");
        }
    }
}
