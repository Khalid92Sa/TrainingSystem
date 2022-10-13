using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingSystem.Application.DTOs.Evaluation;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Service.Services
{
    public class EvaluationService:IEvaluationService
    {
        private IEvaluationRepository _evaluationRepository;

        public EvaluationService(IEvaluationRepository evaluationRepository)
        {
            _evaluationRepository = evaluationRepository;
        }


            public EvaluationDTO GetTrainerWithListOfEvaluationById(int id)
        {
            var result = _evaluationRepository.GetTrainerWithListOFTraineesById(id);
            if (result == null)
            {
                return new EvaluationDTO();
            }
            var evaluationDTO = new EvaluationDTO()
            {
                TrainerID = result.ID,
                TrainerName = result.Name,
                evaluationTraineesDTOs = result.Section.Trainees.Select(t => new EvaluationTraineesDTO()
                {
                    traineeID = t.ID,
                    Name = t.Name,
                    SectionField = t.Section.SectionField.SectionField,
                    EvaluationRate=t.Evaluation==null?0:t.Evaluation.EvaluationRate
                }).ToList()

            };
            return evaluationDTO;
        }

        public TraineeQuestionsDTO getTraineeWithEvaluationForm(int id)
        {
            var question = _evaluationRepository.getAllQuestions();

            var trainee = _evaluationRepository.GetTraineeById(id);

            if (trainee == null)
            {
               
                return new TraineeQuestionsDTO();
            }

            var traineeQuestionDTO = new TraineeQuestionsDTO()
            {
                TraineeID = trainee.ID,
                TraineeName = trainee.Name,
                //EvaluationRate= trainee.Evaluation.EvaluationRate,
                //feedback = trainee.Evaluation.feedback,
                questions =question.Select(item=>new QuestionDto { 
               Id=item.id,
               Question=item.question
                })
            .ToList()
            };  
            return traineeQuestionDTO;

        }

        public void AddEvaluation(evaluationRequestDto evaluation)
        {
            var evaluationRate = evaluation.Questions.Select(item=>item.value).Sum();
            
            var evaluationn = new Evaluation
            {
                TraineeID = evaluation.TraineeID,
                feedback = evaluation.feedback,
                EvaluationRate =evaluationRate,
                
                
            };
            _evaluationRepository.AddEvaluation(evaluationn);
        }







    }
}
