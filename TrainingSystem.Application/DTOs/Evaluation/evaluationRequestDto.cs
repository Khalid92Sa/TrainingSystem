

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrainingSystem.Application.DTOs.Evaluation
{
   public class evaluationRequestDto
    {
      
            public string TrainerID { get; set; }
            public int EvaluationRate { get; set; }
            public string feedback { get; set; }
            public int TraineeID { get; set; }
        public List<QuestionDto> Questions { get; set; }

    }
}
