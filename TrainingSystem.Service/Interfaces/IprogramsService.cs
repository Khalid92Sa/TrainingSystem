using System.Linq;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Service.Interfaces
{
    public interface IprogramsService
    {
        IQueryable<Programs> Programs { get; }
        void AddProgram(Programs programs);
        Task RemoveProgram(Programs programs);
        void RemoveSectionfromProgram(ProgramSection programSection);

        Task<Programs> GetProgramById(string? id);
        void UpdateProgram(Programs programs);
        Task SaveChangesAsyncc();
    }
}
