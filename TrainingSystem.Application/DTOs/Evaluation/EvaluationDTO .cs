using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.DTOs.Evaluation
{
    public class EvaluationDTO
    {
        
        public int TrainerID { get; set; }
        public string TrainerName { get; set; }
        
        public List<EvaluationTraineesDTO> evaluationTraineesDTOs { get; set; }

    }
}
