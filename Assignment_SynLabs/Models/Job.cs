using Microsoft.AspNetCore.Builder;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_SynLabs.Models
{
    public class Job
    {
        [Column("JobId")]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedOn { get; set; }
        public int TotalApplications { get; set; }
        public string CompanyName { get; set; }
        public User PostedBy { get; set; }
    }
}
