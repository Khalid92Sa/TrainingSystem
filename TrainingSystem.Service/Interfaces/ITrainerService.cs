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
        Task<Trainer> GetTrainerById(int id);
        void UpdateTrainer(int id, Trainer trainer);
        bool Login(string email,string password,int TrainerID);
        void LogoutTrainer(int TrainerID);
        public void SendLoginInfo(Trainer trainer);
        public Boolean RepetedName(string Name);
        public Boolean RepetedNameupdate(string Name, int id);

    }
}
