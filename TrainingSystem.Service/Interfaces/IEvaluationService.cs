using System;
using System.Collections.Generic;
using System.Text;
using TrainingSystem.Application.DTOs.Evaluation;
using TrainingSystem.Domain;

namespace TrainingSystem.Service.Interfaces
{
   public interface IEvaluationService
    {
        EvaluationDTO GetTrainerWithListOfEvaluationById(int id);
        TraineeQuestionsDTO getTraineeWithEvaluationForm(int id);
         void AddEvaluation(evaluationRequestDto evaluation);
    }
}
