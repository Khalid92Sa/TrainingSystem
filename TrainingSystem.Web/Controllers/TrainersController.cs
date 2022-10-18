using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
//using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainingSystem.Application.DTOs.Trainers;
using TrainingSystem.Domain;
using TrainingSystem.Service;
using Microsoft.Extensions.Logging;
using TrainingSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;
using System.Xml.Linq;
using AspNetCore.Reporting;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
//using SelectPdf;

namespace TrainingSystem.Web.Controllers
{
    [Authorize]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITrainerService _TrainerService;
        private readonly ISectionLookup _SectionLookup;
        private readonly ISection _section;
        private readonly ISectionLookup RepoSectionLookup;
        private readonly IConfiguration _configuration;

        public TrainersController(ITrainerService trainerService,
            ApplicationDbContext context,
            ISection section,
            ISectionLookup sectionLookup,
            ISectionLookup repoSectionLookup,
            IConfiguration configuratio)
        {
            _TrainerService = trainerService;
            _context = context;
            _section = section;
            _SectionLookup = sectionLookup;
            RepoSectionLookup = repoSectionLookup;
            _configuration = configuratio;

        }

        // GET: Trainersf
        public IActionResult Index(string SearchById, string SearchByName, string SearchByActivation, string SearchBySectionFeild)
        {
            ViewData["SectionField"] = new SelectList(_SectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["searchSection"] = SearchBySectionFeild;
            ViewData["Status"] = SearchByActivation;
            ViewData["SearchById"] = SearchById;
            ViewData["SearchByNmae"] = SearchByName;
            ViewData["SectionField2"] = _SectionLookup.SectionLookUp;
            IQueryable<Trainer> Trainers = _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField);
            if (SearchByName != null)
            {
                Trainers = Trainers.Where(n => n.Name.Contains(SearchByName));
            }
            if (SearchById != null)
            {
                Trainers = Trainers.Where(n => n.ID.ToString().Contains(SearchById));
            }
            if (SearchByActivation != null)
            {
                if (SearchByActivation == "Active")
                {
                    Trainers = Trainers.Where(n => n.Status == true);
                }
                else
                {
                    Trainers = Trainers.Where(n => n.Status == false);
                }
            }
            if (SearchBySectionFeild != null)
            {
                Trainers = Trainers.Where(s => s.SectionLookupID.ToString()==SearchBySectionFeild || s.SectionLookupID1.ToString() == SearchBySectionFeild);
                SectionLookup sectionlookup = _SectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(SearchBySectionFeild));
                ViewData["SectionFieldResult"] = sectionlookup.SectionField;
            }
            return View(Trainers);
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        public IActionResult Create()
        {
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LastName,SectionID,Status,ContactNumber,Email,Password,SectionLookupID,Address")] Trainer trainer)
        {
            trainer.UserName = trainer.Name + '_' + trainer.LastName;
            
            if (ModelState.IsValid && _TrainerService.RepetedName(trainer.UserName))
            {
                
                _TrainerService.AddTrainer(trainer);
                await _TrainerService.SaveChangesAsyncc();
                _TrainerService.SendLoginInfo(trainer);
                return RedirectToAction(nameof(Index));
            }
            if (!_TrainerService.RepetedName(trainer.UserName))
            {
                ViewData["ErrorMessage"] = "This trainer already added.";
            }
            else
            {
                ViewData["ErrorMessage"] = "";
            }
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _TrainerService.GetTrainerById(id);
            if (trainer == null)
            {
                return NotFound();
            }
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,LastName,Status,ContactNumber,Email,SectionLookupID,SectionLookupID1,Address,Password,SectionID")] Trainer trainer)
        {
            trainer.UserName = trainer.Name + '_' + trainer.LastName;
            if (ModelState.IsValid && _TrainerService.RepetedNameupdate(trainer.UserName,id))
            {
                try
                {
                    
                    _TrainerService.UpdateTrainer(id, trainer);
                    await _TrainerService.SaveChangesAsyncc();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            if (!_TrainerService.RepetedNameupdate(trainer.UserName, id))
            {
                ViewData["ErrorMessage"] = "This trainer already exist.";
            }
            else
            {
                ViewData["ErrorMessage"] = "";
            }
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View(trainer);
        }

        // GET: Trainers/Delete/5

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.ID == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.ID == id);
        }
        public IActionResult TrainersReportPDF()
        {
            IQueryable<Trainer> Trainers = _TrainerService.Trainers.Include(s=>s.SectionField);
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("FirstName");
            dt.Columns.Add("LastName");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("Status");
            foreach (var trainee in Trainers)
            {
                if (trainee.Status)
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.LastName, trainee.SectionField.SectionField, "Active");
                }
                else
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.LastName, trainee.SectionField.SectionField, "Inactive");
                }
            }

            var ReportPath=_configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report2.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result = lr.Execute(RenderType.Pdf, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/pdf");

        }
        public IActionResult TrainersReportExcel()
        {
            IQueryable<Trainer> Trainers = _TrainerService.Trainers.Include(s => s.SectionField);
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("FirstName");
            dt.Columns.Add("LastName");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("Status");
            foreach (var trainee in Trainers)
            {
                if (trainee.Status)
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.LastName, trainee.SectionField.SectionField, "Active");
                }
                else
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.LastName, trainee.SectionField.SectionField, "Inactive");
                }
                
            }


            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report2.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result = lr.Execute(RenderType.Excel, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/vnd.ms-excel");

        }
    }
}
