using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Section GetSectionByID(int id)
        {
            return context.Sections.FirstOrDefault(x => x.ID == id);
        }

        public async Task CreateSection(Section section)
        {
            context.Sections.Add(section);
            await context.SaveChangesAsync();
        }

        

        public async Task UpdateSection(Section section, int id)
        {
            var sectionToUpdate = GetSectionByID(id);
            if (sectionToUpdate == null)
            {
                return;
            }
            sectionToUpdate.SectionLookupID = section.SectionLookupID;
            sectionToUpdate.TrainerID = section.TrainerID;
            sectionToUpdate.StartDate = section.StartDate;
            await context.SaveChangesAsync();
        }

        public System.Data.Common.DbConnection Conn()
        {
            return context.Database.GetDbConnection();
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
