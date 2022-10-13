using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public interface IProgramsRepository
    {
        IQueryable<Programs> Programs { get; }
        void AddProgram(Programs programs);
        Task<Programs> GetProgramById(string? id);
        public void UpdateProgram(string id, Programs program);
        Task SaveChangesAsyncc();
    }
}
