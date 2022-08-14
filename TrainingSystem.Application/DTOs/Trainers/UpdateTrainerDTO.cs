using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Application.DTOs.Trainers
{
    public class UpdateTrainerDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string SectionID { get; set; }
        public bool Status { get; set; }
        public int? ContactNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
    }
}
