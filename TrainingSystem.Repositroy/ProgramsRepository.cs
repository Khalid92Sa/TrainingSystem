using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class ProgramsRepository : IProgramsRepository
    {
        private ApplicationDbContext _context;
        public ProgramsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Programs> Programs => _context.Programs;

        public void AddProgram(Programs programs)
        {
            _context.Add(programs);
        }

        public async Task<Programs> GetProgramById(string id)
        {
            var program = await _context.Programs.FindAsync(id);
            return program;
        }

        public async Task SaveChangesAsyncc()
        {
          await  _context.SaveChangesAsync();
        }

        public async void UpdateProgram(string id,Programs program)
        {
            Programs programToUpdate =await _context.Programs.FindAsync(id);
            if (programToUpdate == null)
            {
                return;
            }
            programToUpdate.Name = program.Name;
            programToUpdate.TrainerID = program.TrainerID;
            programToUpdate.StartDate = program.StartDate;
            programToUpdate.EndDate = program.EndDate;
            programToUpdate.Trainer = program.Trainer;
            await _context.SaveChangesAsync();
        }
    }
}
