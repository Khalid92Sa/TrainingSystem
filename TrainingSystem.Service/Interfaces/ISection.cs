using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Service
{
    public interface ISection
    {
        IQueryable<Section> Sections { get; }
        public Section GetSectionByID(string id);
        public void CreateSection(Section section);
        public void UpdateSection(Section section, string id);
    }
}
