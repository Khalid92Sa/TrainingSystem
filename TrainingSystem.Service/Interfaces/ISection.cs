using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;

namespace TrainingSystem.Service
{
    public interface ISection
    {
        IQueryable<Section> Sections { get; }
        public Section GetSectionByID(int id);
        public Task CreateSection(Section section);
        public void SendEvaluationEmail(Section section);
        public void SendStartEmail(Section section);
        public Task UpdateSection(Section section, int id);
    }
}
