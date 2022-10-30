using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingSystem.Domain
{
    public class Trainer
    {
        [Range(0, 5)]
        public int ID { get; set; }

        public string idstring
        {
            get
            {
                if (ID < 10)
                { return "TR-0" + ID; }
                return "TR-" + ID;
            }
        }
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Invalid FirstName, use english letter only")]

        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Invalid LastName, use english letter only")]

        public string LastName { get; set; }
        public bool Status { get; set; }
        public bool Loginstatus { get; set; }
        [Required]
        [RegularExpression(@"^[0-9''-'\s]{1,14}$",
               ErrorMessage = "Invalid Phone number, use english number only")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "This field must be 14 numbers and containes country code")]
        public string ContactNumber { get; set; }
        public string UserName { get; set; }
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Invalid Address, use english letter only")]
        public string Address { get; set; } = "Amman";
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [ForeignKey("Section")]
        public int? SectionID { get; set; }
        public Section Section { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&-]).{8,}$",
         ErrorMessage = "Passwords must be at least 8 characters." +
            "\r\nPasswords must have at least one non alphanumeric character." +
            "\r\nPasswords must have at least one digit ('0'-'9')." +
            "\r\nPasswords must have at least one uppercase ('A'-'Z')."+
            "\r\nPasswords must have at least one lowercase ('a'-'z').")]
        public string Password { get; set; }

        public ICollection<Evaluation> Evaluations { get; set; }
        [Required]
        public int SectionLookupID { get; set; }
        public SectionLookup SectionField { get; set; }
        public int? SectionLookupID1 { get; set; }
        public ICollection<Trainee> Trainees { get; set; }
    }
}