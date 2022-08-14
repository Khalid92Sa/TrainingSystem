using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingSystem.Domain;
using TrainingSystem.Service;

namespace TrainingSystem.Service
{
    public class TraineeService : ITrainee
    {
        private readonly ApplicationDbContext context;
        public TraineeService(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Trainee> Trainees => context.Trainees;

        public Trainee GetTraineeByID(int id)
        {
            return context.Trainees.FirstOrDefault(x => x.ID == id);
        }

        public void CreateTrainee(Trainee trainee)
        {
            context.Trainees.Add(trainee);
            context.SaveChanges();
        }     

        public void UpdateTrainee(Trainee trainee, int id)
        {
            var traineeToUpdate = GetTraineeByID(id);
            if (traineeToUpdate == null)
            {
                return;
            }
            traineeToUpdate.Name = trainee.Name;
            traineeToUpdate.Email = trainee.Email;
            traineeToUpdate.CV = trainee.CV;
            traineeToUpdate.GraduationDate = trainee.GraduationDate;
            context.SaveChanges();
        }
    }
}
