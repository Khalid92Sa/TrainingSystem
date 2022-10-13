using System;
using System.Linq;
using System.Threading.Tasks;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Service.Services
{
    public class ProgramsService : IprogramsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProgramsRepository _ProgramsRepository;

        public ProgramsService(IProgramsRepository programsRepository, ApplicationDbContext context)
        {
            _ProgramsRepository = programsRepository;
            _context = context;
        }
        public IQueryable<Programs> Programs => _ProgramsRepository.Programs;

        public void AddProgram(Programs programs)
        {
            _ProgramsRepository.AddProgram(programs);
        }

        public async Task<Programs> GetProgramById(string id) => await _ProgramsRepository.GetProgramById(id);

        public async Task RemoveProgram(Programs programs)
        {
            _context.Programs.Remove(programs);
            await SaveChangesAsyncc();
        }

        public void RemoveSectionfromProgram(ProgramSection programSection)
        {
            _context.programSections.Remove(programSection);

        }

        public bool RepetedName(string Name)
        {
            var programes = _context.Programs.FirstOrDefault(x => x.Name == Name);
            if (programes == null)
            {
                return true;
            }
            return false;
        }

        public bool RepetedNameupdate(string Name, string id)
        {
            var programToUpdate= _context.Programs.FirstOrDefault(x => x.ID == id);
            if (programToUpdate.Name == Name)
            {
                return true;
            }
            var trainees = _context.Programs.FirstOrDefault(x => x.Name == Name);
            if (trainees == null)
            {
                return true;
            }
            return false;
        }
        public async Task SaveChangesAsyncc()
        {
            await _ProgramsRepository.SaveChangesAsyncc();
        }

        public void UpdateProgram(string id,Programs programs)
        {
            _ProgramsRepository.UpdateProgram(id,programs);
        }

    }
}
