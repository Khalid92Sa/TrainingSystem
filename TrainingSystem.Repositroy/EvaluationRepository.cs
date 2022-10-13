using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class EvaluationRepository:IEvaluationRepository
    {
        private ApplicationDbContext _context;
        public EvaluationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Trainer GetTrainerWithListOFTraineesById(int id)
        {
            
            return _context.Trainers
                .Include(b => b.Section)
                .ThenInclude(b => b.Trainees)
                 .ThenInclude(b => b.Evaluation)
                .Include(b => b.Section)
                    .ThenInclude(b => b.SectionField)
                .SingleOrDefault(s => s.ID == id);

        }
        
        public IQueryable<Questions> getAllQuestions()
        {
            return _context.Questions;
             
        }

        public Trainee GetTraineeById(int id)
        {
            return _context.Trainees.FirstOrDefault(t => t.ID == id);
        }

        public void AddEvaluation(Evaluation evaluation)
        {
            _context.Evaluations.Add(evaluation);
            _context.SaveChanges();
        }

    }
}
