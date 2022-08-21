using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingSystem.Domain;

namespace TrainingSystem.Service.Interfaces
{
    public interface ISectionLookup
    {
        IQueryable<SectionLookup> SectionLookUp { get; }
    }
}
