using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Trainers;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class TrainerRepository : ITrainerRepository
    {
        private ApplicationDbContext _context;
        public TrainerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Trainer> Trainers => _context.Trainers;

        public async Task SaveChangesAsyncc()
        {
            await _context.SaveChangesAsync();
        }
        public void AddTrainer(Trainer trainer)
        {
             _context.Add(trainer);
        }
        public async Task<Trainer> GetTrainerById(string? id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            return trainer;
        }
        public void UpdateTrainer(Trainer trainer)
        {
            _context.Update(trainer);
        }
    }
}
