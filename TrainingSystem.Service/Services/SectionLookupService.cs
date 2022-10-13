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

        public async Task CreateSectionField(string sectionfield)
        {
            SectionLookup sectionLookup = new SectionLookup();
            sectionLookup.SectionField = sectionfield;
            context.SectionLookup.Add(sectionLookup);
            await context.SaveChangesAsync();
        }
    }
}
