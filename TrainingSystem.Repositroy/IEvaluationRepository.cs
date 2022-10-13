using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public interface IEvaluationRepository
    {
        Trainer GetTrainerWithListOFTraineesById(int id);
        IQueryable<Questions> getAllQuestions();
        Trainee GetTraineeById(int id);
        void AddEvaluation(Evaluation evaluation);
      
    }
}
