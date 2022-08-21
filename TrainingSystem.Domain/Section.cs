using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrainingSystem.Domain
{
    public class Section
    {
        [Range (1, 5)]
        public string ID { get; set; }

        [Required]
        public string SectionField { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime EndDate {
            get{return StartDate.AddMonths(3);}
        }
        public ICollection<Trainer> trainers { get; set; }
        public ICollection<ProgramSection> programSections { get; set; }

    }
}
