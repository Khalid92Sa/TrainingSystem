using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingSystem.Domain;
using TrainingSystem.Service;

namespace TrainingSystem.Service
{
    public class SectionService : ISection
    {
        private readonly ApplicationDbContext context;
        public SectionService(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Section> Sections =>
            context.Sections;

        public Section GetSectionByID(string id)
        {
            return context.Sections.FirstOrDefault(x => x.ID == id);
        }

        public void CreateSection(Section section)
        {
            context.Sections.Add(section);
            context.SaveChanges();
        }

        

        public void UpdateSection(Section section, string id)
        {
            var sectionToUpdate = GetSectionByID(id);
            if (sectionToUpdate == null)
            {
                return;
            }
            sectionToUpdate.SectionField = section.SectionField;
            sectionToUpdate.StartDate = section.StartDate;
            context.SaveChanges();
        }

        
    }
}
