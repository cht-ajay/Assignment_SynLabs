using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_SynLabs.Models
{
    public class User
    {
            [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public UserType UserType { get; set; }
            public string PasswordHash { get; set; }
            public string ProfileHeadline { get; set; }
            public Profile Profile { get; set; }
        

      
    }
    public enum UserType
    {
        Applicant,
        Admin
    }
}
