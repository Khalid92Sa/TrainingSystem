using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Trainers;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public interface ITrainerRepository
    {
        IQueryable<Trainer> Trainers { get; }
        Task SaveChangesAsyncc();
        void AddTrainer(Trainer trainer);
        Task<Trainer> GetTrainerById(string id);
        void UpdateTrainer(Trainer trainer);
        bool Login(string email, string password);
    }
}
