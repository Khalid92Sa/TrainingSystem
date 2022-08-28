using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class ViewModelReporting
    {
        public string Trainee { get; set; }
        public string Trainer { get; set; }
        public string Sectionfield { get; set; }
        public bool Status { get; set; }
        public int Evaluation { get; set; }
        public bool IsInSection { get; set; }

    }
}
