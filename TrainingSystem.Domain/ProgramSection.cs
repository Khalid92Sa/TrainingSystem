using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Domain
{
    public class ProgramSection
    {
        public int SectionID { get; set; }
        public string ProgramsID { get; set; }
        public Programs programs { get; set; }
        public Section section { get; set; }
    }
}
