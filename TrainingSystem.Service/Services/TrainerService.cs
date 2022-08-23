using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Trainers;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;

namespace TrainingSystem.Service
{
    public class TrainerService : ITrainerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITrainerRepository _TrainerRepository;

        public TrainerService(ITrainerRepository trainerRepository , ApplicationDbContext context)
        {
            _TrainerRepository = trainerRepository;
            _context = context;
        }

        public IQueryable<Trainer> Trainers => _TrainerRepository.Trainers;

        public  void AddTrainer(Trainer trainer)
        {
             _TrainerRepository.AddTrainer(trainer);
            
        }

        public async Task SaveChangesAsyncc()
        {
            await _TrainerRepository.SaveChangesAsyncc();
        }
        public async Task<Trainer> GetTrainerById(string id)=>await _TrainerRepository.GetTrainerById(id);
        public void UpdateTrainer(Trainer trainer)
        {
              _TrainerRepository.UpdateTrainer(trainer);

             
        }

        public bool Login(string email, string password)
        {
            var trainer =  _context.Trainers.FirstOrDefault(s=>s.Name== email && s.Password== password);
            if(trainer == null)
            { return false; }
            else { return true; }
        }
    }
}
