using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Domain
{
    public class SectionLookup
    {
        public int SectionLookupID { get; set; }
        public string SectionField { get; set; }

        public ICollection<Section> Sections { get; set; }

    }
}
