using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrainingSystem.Domain;

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
            var sectionupdateTrainer = context.Sections.Max(i => i.ID);
            Trainer trainer = context.Trainers.First(s => s.ID == section.TrainerID);
            trainer.SectionID = sectionupdateTrainer;
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
            sectionToUpdate.StartDate = section.StartDate;


            if (section.TrainerID != sectionToUpdate.TrainerID)
            {
                Trainer AddSectionIdTrainer = context.Trainers.First(s => s.ID == section.TrainerID);
                Trainer RemoveSectionIdTrainer = context.Trainers.First(s => s.ID == sectionToUpdate.TrainerID);
                AddSectionIdTrainer.SectionID = sectionToUpdate.ID;
                RemoveSectionIdTrainer.SectionID = null;
            }
            sectionToUpdate.TrainerID = section.TrainerID;
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
