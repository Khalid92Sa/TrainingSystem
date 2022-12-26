using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        public ProgramsController(ApplicationDbContext context,
            ISection section,
            IprogramsService program,
            ITrainerService trainer,
            IConfiguration configuration
            )
        {
            _context = context;
            _section = section;
            _program = program;
            _trainer = trainer;
            _configuration = configuration;
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
                applicationDbContext = applicationDbContext.Where(s => s.TrainerID.ToString().Contains(HeadOfProgram));
                var head = _trainer.Trainers.First(s => s.ID.ToString() == HeadOfProgram);
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
        public async Task<IActionResult> Create([Bind("ID,Name,TrainerID,StartDate,EndDate")] Programs programs, int[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                programs.programSections = new List<ProgramSection>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new ProgramSection { ProgramsID = programs.ID, SectionID = course };
                    programs.programSections.Add(courseToAdd);

                }
            }
            if (ModelState.IsValid && _program.RepetedName(programs.Name))
            {

                _program.AddProgram(programs);
                await _program.SaveChangesAsyncc();
                return RedirectToAction(nameof(Index));
            }
            if (!_program.RepetedName(programs.Name))
            {
                ViewData["ErrorMessage"] = "Name is already exist";
            }
            else
            {
                ViewData["ErrorMessage"] = null;
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
        public async Task<IActionResult> Edit(string id, [Bind("ID,Name,TrainerID,StartDate,EndDate")] Programs programs, string[] selectedCourses)
        {
          
            var ProgramToUpdate = await _program.Programs
                .Include(i => i.programSections)
                .Include(i => i.programSections)
                    .ThenInclude(i => i.section)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (programs.TrainerID != 0&&  programs.StartDate != null && programs.Name != null && _program.RepetedNameupdate(programs.Name, id))
            {
                _program.UpdateProgram(id, programs);
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
            if (!_program.RepetedNameupdate(programs.Name, id))
            {
                ViewData["ErrorMessage"] = "Name is already exist";
            }
            else
            {
                ViewData["ErrorMessage"] = null;
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
            IQueryable<Section> allSection = _section.Sections
                .Include(x => x.Trainer)
                .Include(s => s.SectionField)
                .Include(s=>s.ProgramSection)
                .Where(s=>s.ProgramSection == null || s.ProgramSection.ProgramsID == programs.ID);
            var ProgramSection = new HashSet<int>(programs.programSections.Select(c => c.SectionID));
            var viewModel = new List<AssignedSectionData>();
            var viewModell = new List<int>();
            foreach (var section in allSection)
            {
               
                    Trainer trainer = _trainer.Trainers.FirstOrDefault(s => s.ID == section.TrainerID);
                    section.Trainer = trainer;
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

            var Programs =await _program.Programs
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
            foreach (var program in Programs.programSections)
            {

                    Trainer trainer = _trainer.Trainers.FirstOrDefault(s => s.ID == program.section.TrainerID);
                    program.section.Trainer = trainer;
                
                
            }
            if (Programs == null)
            {
                return NotFound();
            }

            return View(Programs);
        }
        private bool ProgramsExists(string id)
        {
            return _program.Programs.Any(e => e.ID == id);
        }
        public IActionResult ProgramsReportPDF()
        {
            IQueryable<Programs> programes = _program.Programs.Include(p => p.Trainer);
            DataTable dt = new DataTable();

            dt.Columns.Add("Name");
            dt.Columns.Add("HeadOfProgram");
            dt.Columns.Add("StartDate");
            dt.Columns.Add("EndDate");
            foreach (var programe in programes)
            {

               dt.Rows.Add(programe.Name, programe.Trainer.Name, programe.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), programe.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            }


            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report4.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result = lr.Execute(RenderType.Pdf, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/pdf");

        }
        public IActionResult ProgramsReportExcel()
        {
            IQueryable<Programs> programes = _program.Programs.Include(p => p.Trainer);
            DataTable dt = new DataTable();

            dt.Columns.Add("Name");
            dt.Columns.Add("HeadOfProgram");
            dt.Columns.Add("StartDate");
            dt.Columns.Add("EndDate");
            foreach (var programe in programes)
            {

                dt.Rows.Add(programe.Name, programe.Trainer.Name, programe.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), programe.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            }

            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report4.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var result = lr.Execute(RenderType.Excel, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/vnd.ms-excel");
        }
    }
}
