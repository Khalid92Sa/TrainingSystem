using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
