using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.DTOs.Evaluation
{
    public class EvaluationTraineesDTO
    {
      
        public int traineeID { get; set; }
        public string Name { get; set; }
        public string SectionField { get; set; }
        public int EvaluationRate { get; set; }

    }
}
