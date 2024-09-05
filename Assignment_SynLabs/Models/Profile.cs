using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_SynLabs.Models
{
    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ResumeFileAddress { get; set; }
        public string Skills { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? ApplicantId { get; set; }
        public User? Applicant { get; set; }
    }
}
