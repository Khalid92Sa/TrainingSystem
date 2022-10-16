using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Service.Services
{
    public class SectionLookupService : ISectionLookup
    {
        private readonly ApplicationDbContext context;
        public SectionLookupService(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<SectionLookup> SectionLookUp => context.SectionLookup;

        public async Task CreateSectionField(string sectionfield,string Year)
        {
            SectionLookup sectionLookup = new SectionLookup();
            sectionLookup.SectionField = sectionfield;
            sectionLookup.Year= Year;
            context.SectionLookup.Add(sectionLookup);
            await context.SaveChangesAsync();
        }

        public async Task EditSectionField(int id, string sectionfield, string Year)
        {
            SectionLookup SectionField = SectionLookUp.FirstOrDefault(s => s.SectionLookupID == id);
            SectionField.SectionField = sectionfield;
            SectionField.Year = Year;
            await context.SaveChangesAsync();
        }
    }
}
