using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace TrainingSystem.Domain
{
    public class Trainee
    {
        [Range(1, 5)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public DateTime EndDate {
            get { return StartDate.AddMonths(3);}
        }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public DateTime GraduationDate { get; set; }

        public byte[] CV { get; set; }

        public string CVFileName { get; set; }
        [Required]
        public bool GraduationStatus { get; set; }
    }
}
