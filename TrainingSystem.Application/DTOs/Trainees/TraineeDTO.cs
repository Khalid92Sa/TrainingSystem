using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.DTOs.Trainees
{
    public class TraineeDTO
    {
        [Range(0, 5)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }



        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime GraduationDate { get; set; }

        public IFormFile CV { get; set; }

        public string CVFileName { get; set; }
        public int SectionID { get; set; }
        
        public string TrainerID { get; set; }
        
    }
}
