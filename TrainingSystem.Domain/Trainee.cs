using System;
using System.ComponentModel.DataAnnotations;

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
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int ContactNumber { get; set; }

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
        public int? SectionID { get; set; }
        public Section Section { get; set; }
        public string TrainerID { get; set; }
        public Trainer Trainer { get; set; }
    }
}
