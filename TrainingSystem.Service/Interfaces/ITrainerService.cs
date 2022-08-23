using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Trainers;
using TrainingSystem.Domain;

namespace TrainingSystem.Service
{
    public interface ITrainerService
    {
        IQueryable<Trainer> Trainers { get; }
        void AddTrainer(Trainer trainer);
        Task SaveChangesAsyncc();
        Task<Trainer> GetTrainerById(string id);
        void UpdateTrainer(Trainer trainer);
        bool Login(string email,string password);

    }
}
