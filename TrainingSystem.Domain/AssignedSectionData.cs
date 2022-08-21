using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Domain
{
    public class AssignedSectionData
    {
        public string SectionID { get; set; }
        public string SectionField { get; set; }
        public ICollection<Trainer> TrainerName { get; set; }
        public bool Assigned { get; set; }
    }
}
