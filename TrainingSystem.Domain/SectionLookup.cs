using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Domain
{
    public class SectionLookup
    {
        public int SectionLookupID { get; set; }
        [Required]
        public string SectionField { get; set; }

        public ICollection<Section> Sections { get; set; }
        public ICollection<Trainee> Trainees { get; set; }
        public ICollection<Trainer> Trainers { get; set; }

    }
}
