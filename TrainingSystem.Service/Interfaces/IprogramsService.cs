using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Service.Interfaces
{
    public interface IprogramsService
    {
        IQueryable<Programs> Programs { get; }
        void AddProgram(Programs programs);
        Task<Programs> GetProgramById(string? id);
        void UpdateProgram(Programs programs);
        Task SaveChangesAsyncc();
    }
}
