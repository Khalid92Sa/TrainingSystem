using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Domain
{
    public class AssignedSectionData
    {
        public int SectionID { get; set; }
        public string SectionField { get; set; }
        public Trainer TrainerName { get; set; }
        public bool Assigned { get; set; }
    }
}
