using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task SaveChangesAsyncc()
        {
            await _ProgramsRepository.SaveChangesAsyncc();
        }

        public void UpdateProgram(Programs programs)
        {
            _ProgramsRepository.UpdateProgram(programs);
        }
    }
}
