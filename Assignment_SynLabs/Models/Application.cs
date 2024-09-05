namespace Assignment_SynLabs.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public int ApplicantId { get; set; }
        public User Applicant { get; set; }
    }


}
