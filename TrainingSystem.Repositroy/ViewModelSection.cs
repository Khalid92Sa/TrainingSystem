using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class ViewModelSection
    {
        public IQueryable<Section> Sections { get; set; }
        public List<SectionsFields> SectionsFields { get; set; }
    }
}
