using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingSystem.Domain;
using TrainingSystem.Service;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Web.Controllers
{
    [Authorize]
    public class ProgramsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISection _section;
        private readonly IprogramsService _program;
        private readonly ITrainerService _trainer;
        public ProgramsController(ApplicationDbContext context,
            ISection section,
            IprogramsService program,
            ITrainerService trainer
            )
        {
            _context = context;
            _section = section;
            _program = program;
            _trainer = trainer;
        }

        // GET: Programs
        public async Task<IActionResult> Index(string SearchStringByName, string DateFilter, string HeadOfProgram)
        {
            ViewData["HeadOfProgram"] = new SelectList(_trainer.Trainers, "ID", "Name");
            ViewData["searchHeadOfProgram"] = HeadOfProgram;
            ViewData["CurrentFilterDate"] = DateFilter;
            ViewData["SearchByName"] = SearchStringByName;
            IQueryable<Programs> applicationDbContext = _program.Programs.Include(p => p.Trainer);
            if (SearchStringByName != null)
            {
                applicationDbContext = applicationDbContext.Where(s => s.Name.Contains(SearchStringByName));
            }
            if (DateFilter != null)
            {
                applicationDbContext = applicationDbContext.Where(s => s.StartDate.ToString().Contains(DateFilter));
            }
            if (HeadOfProgram != null)
            {
                applicationDbContext = applicationDbContext.Where(s => s.TrainerID.Contains(HeadOfProgram));
                var head = _trainer.Trainers.First(s => s.ID == HeadOfProgram);
                ViewData["HeadOfProgramResult"] = head.Name;
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Programs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var programs = await _context.Programs
            //    .Include(p => p.Trainer)
            //    .FirstOrDefaultAsync(m => m.ID == id);
            var programs = await _program.Programs
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (programs == null)
            {
                return NotFound();
            }

            return View(programs);
        }

        // GET: Programs/Create
        public IActionResult Create()
        {
            ViewData["TrainerID"] = new SelectList(_trainer.Trainers, "ID", "Name");
            var programs = new Programs();
            programs.programSections = new List<ProgramSection>();
            PopulateAssignedSectionData(programs);
            return View();
        }

        // POST: Programs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,TrainerID,StartDate")] Programs programs, int[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                programs.programSections = new List<ProgramSection>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new ProgramSection { ProgramsID = programs.ID, SectionID = course };
                    //to test
                    //foreach (var x in _context.progSecs.Where(x => x.SectionID == course))
                    //{
                    //    x.section.StardDate = programs.StartDate;
                    //}
                    programs.programSections.Add(courseToAdd);

                }
            }
            if (ModelState.IsValid)
            {
                //_context.Add(programs);
                //await _context.SaveChangesAsync();
                _program.AddProgram(programs);
                await _program.SaveChangesAsyncc();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerID"] = new SelectList(_trainer.Trainers, "ID", "Name", programs.TrainerID);
            PopulateAssignedSectionData(programs);
            return View(programs);
        }

        // GET: Programs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programs = await _program.Programs
                .Include(i => i.programSections)
                .Include(i => i.programSections).ThenInclude(i => i.section)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (programs == null)
            {
                return NotFound();
            }
            ViewData["TrainerID"] = new SelectList(_trainer.Trainers, "ID", "Name", programs.TrainerID);
            PopulateAssignedSectionData(programs);
            return View(programs);
        }

        // POST: Programs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Name,TrainerID,StartDate")] Programs programs, string[] selectedCourses)
        {
            if (id != programs.ID)
            {
                return NotFound();
            }

            var ProgramToUpdate = await _program.Programs
                .Include(i => i.programSections)
                .Include(i => i.programSections)
                    .ThenInclude(i => i.section)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (await TryUpdateModelAsync<Programs>(
                 ProgramToUpdate,
                 "",
                 x => x.Name, x => x.StartDate, x => x.EndDate, x => x.TrainerID, x => x.Trainer))
            {
                UpdateInstructorCourses(selectedCourses, ProgramToUpdate);
                try
                {
                    await _program.SaveChangesAsyncc();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerID"] = new SelectList(_trainer.Trainers, "ID", "Name", programs.TrainerID);
            UpdateInstructorCourses(selectedCourses, ProgramToUpdate);
            PopulateAssignedSectionData(ProgramToUpdate);
            return View(programs);
        }

        // GET: Programs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programs = await _program.Programs
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (programs == null)
            {
                return NotFound();
            }

            return View(programs);
        }

        // POST: Programs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var programs = await _program.Programs.FirstOrDefaultAsync(m => m.ID == id);
            await _program.RemoveProgram(programs);
            await _program.SaveChangesAsyncc();
            return RedirectToAction(nameof(Index));
        }







        private void PopulateAssignedSectionData(Programs programs)
        {
            var allSection = _section.Sections.Include(x => x.Trainer).Include(s => s.SectionField);
            var ProgramSection = new HashSet<int>(programs.programSections.Select(c => c.SectionID));
            var viewModel = new List<AssignedSectionData>();
            var viewModell = new List<int>();
            foreach (var section in allSection)
            {

                viewModel.Add(new AssignedSectionData
                {
                    SectionID = section.ID,
                    SectionField = section.SectionField.SectionField,
                    TrainerName = section.Trainer,
                    Assigned = ProgramSection.Contains(section.ID)
                });

            }
            ViewData["Courses"] = viewModel;
        }





        private async void UpdateInstructorCourses(string[] selectedCourses, Programs programToUpdate)
        {
            if (selectedCourses == null)
            {
                programToUpdate.programSections = new List<ProgramSection>();
                return;
            }

            var selectedSectionHS = new HashSet<string>(selectedCourses);
            var programsection = new HashSet<int>
                (programToUpdate.programSections.Select(c => c.section.ID));
            foreach (var course in _section.Sections)
            {
                if (selectedSectionHS.Contains(course.ID.ToString()))
                {
                    if (!programsection.Contains(course.ID))
                    {
                        course.StartDate = programToUpdate.StartDate;
                        programToUpdate.programSections.Add(new ProgramSection { ProgramsID = programToUpdate.ID, SectionID = course.ID });
                    }
                }
                else
                {

                    if (programsection.Contains(course.ID))
                    {
                        ProgramSection sectionToRemove = programToUpdate.programSections.FirstOrDefault(i => i.SectionID == course.ID);
                        //_context.Remove(sectionToRemove);
                        _program.RemoveSectionfromProgram(sectionToRemove);
                        await _program.SaveChangesAsyncc();
                    }
                }
            }
        }
        public async Task<IActionResult> ViewSection(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var programs = await _program.Programs
                .Include(p => p.programSections)
                    .ThenInclude(x => x.section)
                    .ThenInclude(x => x.SectionField)
                .Include(p => p.programSections)
                    .ThenInclude(x => x.section)
                    .ThenInclude(x => x.Trainer)
                .Include(p => p.programSections)
                    .ThenInclude(x => x.section)
                    .ThenInclude(x => x.Trainees)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (programs == null)
            {
                return NotFound();
            }

            return View(programs);
        }
        private bool ProgramsExists(string id)
        {
            return _program.Programs.Any(e => e.ID == id);
        }
    }
}
