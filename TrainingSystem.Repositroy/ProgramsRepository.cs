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

        public void UpdateProgram(Programs programs)
        {
            _context.Update(programs);
        }
    }
}
