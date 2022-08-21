using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Repositroy
{
    public class TraineesInSection
    {
        public int TraineeID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
        public bool IsInOtherSection { get; set; }
    }
}
