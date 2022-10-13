using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class VMCETrainee
    {
        
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Invalid Name, use english letter only")]
        public string Name { get; set; }
        public int SectionLookupID{get; set; }
        public int TrainerID { get; set; }
        [Required]
        [RegularExpression(@"^[0-9''-'\s]{1,14}$",
               ErrorMessage = "Invalid Phone number, use english number only")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "This field must be 14 numbers and containes country code")]
        public string ContactNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime StartDate
        {
            get { return DateTime.Now; }
        }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime GraduationDate { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string CVFileName { get; set; }

        public IFormFile File { get; set; }
    }
}
