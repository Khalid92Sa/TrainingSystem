using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingSystem.Domain;

namespace TrainingSystem.Service
{
    public interface ITrainee
    {
        IQueryable<Trainee> Trainees { get; }
        public Trainee GetTraineeByID(int id);
        public void CreateTrainee(Trainee trainee);
        public void UpdateTrainee(Trainee trainee, int id);
    }
}
