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
        public async Task<Trainer> GetTrainerById(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            return trainer;
        }
        public void UpdateTrainer(int id,Trainer trainer)
        {
            var trainerToUpdate = _context.Trainers.Find(id);
            if (trainerToUpdate == null)
            {
                return;
            }
            trainerToUpdate.Name = trainer.Name;
            trainerToUpdate.SectionLookupID = trainer.SectionLookupID;
            trainerToUpdate.SectionLookupID1 = trainer.SectionLookupID1;
            trainerToUpdate.Address = trainer.Address;
            trainerToUpdate.UserName = trainer.UserName;
            trainerToUpdate.Email = trainer.Email;
            trainerToUpdate.LastName = trainer.LastName;
            trainerToUpdate.ContactNumber = trainer.ContactNumber;
            trainerToUpdate.Status = trainer.Status;
            //_context.Update(trainer);
        }

        public bool Login(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
