using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Service.Interfaces
{
    public interface ISectionLookup
    {
        IQueryable<SectionLookup> SectionLookUp { get; }
        public Task CreateSectionField(string sectionfield,string Year);
        public Task EditSectionField(int id,string sectionfield, string Year);
    }
}
