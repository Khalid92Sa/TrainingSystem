using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.DTOs.Evaluation
{
   public class TraineeQuestionsDTO
    {
        public int TraineeID { get; set; }
        public string TraineeName { get; set; }
        public string feedback { get; set; }
        [Range(1,100)]
        public int EvaluationRate { get; set; }
        public List<QuestionDto> questions { get; set; }

        public string TrainerID { get; set; }


    }
}
