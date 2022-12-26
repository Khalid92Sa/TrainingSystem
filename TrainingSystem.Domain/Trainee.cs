using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingSystem.Domain
{
    public class Trainee
    {
        [Range(0, 5)]
        public int ID { get; set; }

        public string idstring
        {
            get
            {
                if (ID < 10)
                { return "TE-0" + ID; }
                return "TE-" + ID;
            }
        }
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Invalid Name, use english letter only")]

        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^[0-9''-'\s]{1,14}$",
               ErrorMessage = "Invalid Phone number, use english number only")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "This field must be 14 numbers and containes country code")]
        public string ContactNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime EndDate
        {
            get { return StartDate.AddMonths(3); }
        }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime GraduationDate { get; set; }

        public byte[] CV { get; set; }

        public string CVFileName { get; set; }
        public bool GraduationStatus
        {
            get
            {
                if (GraduationDate < DateTime.Now)
                { return true; }
                else { return false; }
            }

        }
        public string HRFeedback { get; set; }
        public int? SectionID { get; set; }
        public Section Section { get; set; }
        public Evaluation Evaluation { get; set; }
        [ForeignKey("SectionLookup")]
        public int SectionLookupID { get; set; }
        public SectionLookup SectionField { get; set; }
        public int TrainerID { get; set; }
        public Trainer Trainer { get; set; }
    }
}
