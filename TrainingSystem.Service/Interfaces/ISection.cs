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
        public Section GetSectionByID(int id);
        public Task CreateSection(Section section);
        public Task UpdateSection(Section section, int id);
        public System.Data.Common.DbConnection Conn();
        public Task Save();
    }
}
