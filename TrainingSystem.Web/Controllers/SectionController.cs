using AspNetCore.Reporting;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Web.Controllers
{
    [Authorize]
    public class SectionController : Controller
    {
        private readonly ITrainee RepoTrainee;
        private readonly ISection RepoSection;
        private readonly ISectionLookup RepoSectionLookup;
        private readonly ITrainerService RepoTrainer;
        private readonly IConfiguration _configuration;
        public SectionController(
            ITrainee repoTrainee,
            ISection repoSection,
            ISectionLookup repoSectionLookup,
            ITrainerService repoTrainer,
            IConfiguration configuration
            )
        {
            RepoTrainee = repoTrainee;
            RepoSection = repoSection;
            RepoSectionLookup = repoSectionLookup;
            RepoTrainer = repoTrainer;
            _configuration = configuration;
        }
        public ActionResult Index(string searchSection, string DateFilter, string DateFilterFields)
        {
            ViewData["CurrentFilterDateFields"] = DateFilterFields;
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["searchSection"] = searchSection;
            ViewData["CurrentFilterDate"] = DateFilter;
            ViewData["SectionField"] = new SelectList( RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            IQueryable<Section> sections = RepoSection.Sections
                    .Include(s => s.Trainer)
                    .Include(s => s.Trainees)
                    .Include(s => s.SectionField)
                    .Include(s=>s.ProgramSection)
                    .ThenInclude(s=>s.programs);
            foreach(var section in sections)
            {
                Trainer trainer = RepoTrainer.Trainers.FirstOrDefault(s=>s.ID==section.TrainerID);
                section.Trainer = trainer;
            }
            List<SectionsFields> groups = new List<SectionsFields>();
            
            var test = RepoSectionLookup.SectionLookUp;
            foreach (var field in test)
            {
                var row = new SectionsFields { ID = field.SectionLookupID, SectionField = field.SectionField, Year = field.Year };
                groups.Add(row);
            }
            if (searchSection != null)
            {
                sections = sections
                    .Where(s => s.SectionLookupID.ToString()== searchSection);
                SectionLookup sectionlookup = RepoSectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString()==searchSection);
                ViewData["SectionFieldResult"] = sectionlookup.SectionField;
            }
            if (DateFilter != null)
            {
                sections = sections
                    .Where(s => s.StartDate.ToString().Contains(DateFilter));
            }
            if (groups.Count > 0 && DateFilterFields != null)
            {
                groups = (List<SectionsFields>)groups.Where(s => s.Year.ToString().Contains(DateFilterFields)).ToList();
            }
            var ViewModelSectio = new ViewModelSection
            {
                Sections = sections,
                SectionsFields = groups
            };
            return View(ViewModelSectio);
        }

        public ActionResult CreateEdit(int? id, int SectionLookupID,int TrainerID)
        {
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["CreateOrEdit"] = "Edit Section";


            if (id == null)
            {
                PopulateTrainee(SectionLookupID, TrainerID);
                if (SectionLookupID > 0)
                {
                    ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s =>  (s.SectionLookupID == SectionLookupID ||s.SectionLookupID1==SectionLookupID) && s.Status == true), "ID", "Name");
                }


                ViewData["CreateOrEdit"] = "Add new Section";
                return View();
            }

            var section = RepoSection.Sections
                .Include(s => s.Trainer)
                    .Include(s => s.Trainees)
                    .Include(s => s.SectionField).FirstOrDefault(s => s.ID == id);
            PopulateTraineeData(section,SectionLookupID, TrainerID);
            if (SectionLookupID > 0)
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => (s.SectionLookupID == SectionLookupID || s.SectionLookupID1 == SectionLookupID) &&  s.Status == true ), "ID", "Name");
            }
            else
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionLookupID == section.SectionLookupID || s.SectionLookupID1 == SectionLookupID), "ID", "Name");
            }
            return View(section);
        }
        private void PopulateTrainee(int SectionLookupID,int TrainerID)
        {
            IQueryable<Trainee> allTrainee = RepoTrainee.Trainees.Include(s => s.SectionField).Include(s=>s.Trainer);
            if (SectionLookupID > 0)
            {
                allTrainee = allTrainee.Where(s => s.SectionLookupID==SectionLookupID);
            }
            if(TrainerID > 0)
            {
                allTrainee = allTrainee.Where(s => s.TrainerID == TrainerID);
            }
            var viewModel = new List<TraineesInSection>();
            foreach (var trainee in allTrainee)
            {
                viewModel.Add(new TraineesInSection
                {
                    TraineeID = trainee.ID,
                    TrainerName=trainee.Trainer.Name,
                    Name = trainee.Name,
                    SectionField = trainee.SectionField.SectionField,
                    Assigned = false,
                    IsInOtherSection = trainee.SectionID != null
                });
            }
            ViewData["Trainees"] = viewModel;
        }
        private void PopulateTraineeData(Section section,int SectionLookupID,int TrainerID)
        {
            IQueryable<Trainee> allTrainee = RepoTrainee.Trainees.Include(s => s.SectionField).Include(s=>s.Trainer);
            if (SectionLookupID > 0)
            {
                allTrainee = allTrainee.Where(s => s.SectionLookupID == SectionLookupID);
            }
            if (TrainerID > 0)
            {
                allTrainee = allTrainee.Where(s => s.TrainerID == TrainerID);
            }
            else
            {
                allTrainee = allTrainee.Where(s => s.TrainerID == section.TrainerID);
            }
            var Traineeinsection = section.Trainees.Where(S => S.SectionID == section.ID).Select(s => s.ID);
            var viewModel = new List<TraineesInSection>();
            foreach (var trainee in allTrainee)
            {
                viewModel.Add(new TraineesInSection
                {
                    TraineeID = trainee.ID,
                    TrainerName = trainee.Trainer.Name,
                    Name = trainee.Name,
                    SectionField = trainee.SectionField.SectionField,
                    Assigned = Traineeinsection.Contains(trainee.ID),
                    IsInOtherSection = trainee.SectionID != null
                });
            }
            ViewData["Trainees"] = viewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEdit([Bind("SectionLookupID,StartDate,TrainerID")] Section section, int? id, int[] selectedTrainees)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    //Create
                    await RepoSection.CreateSection(section);
                    Section Section = await RepoSection.Sections
                .Include(s => s.Trainer)
                .Include(s => s.Trainees)
                .Include(s => s.SectionField)
                .FirstOrDefaultAsync(s => s.ID == section.ID);
                    if (selectedTrainees != null)
                    {
                        foreach (var trainee in selectedTrainees)
                        {
                            RepoTrainee.AddTraineeToSection(trainee, Section.ID);
                        }
                    }
                    var starttime = Section.StartDate - DateTime.Now;
                    var endtime = Section.EndDate - DateTime.Now;
                    if (starttime.TotalHours > 0 && endtime.TotalHours > 0)
                    {
                        BackgroundJob.Schedule(() => RepoSection.SendStartEmail(Section), TimeSpan.FromHours(starttime.TotalHours));
                        BackgroundJob.Schedule(() => RepoSection.SendEvaluationEmail(Section), TimeSpan.FromHours(endtime.TotalHours));
                    }
                    else if (starttime.TotalHours < 0 && endtime.TotalHours > 0)
                    {
                        BackgroundJob.Schedule(() => RepoSection.SendStartEmail(Section), TimeSpan.FromMinutes(1));
                        BackgroundJob.Schedule(() => RepoSection.SendEvaluationEmail(Section), TimeSpan.FromHours(endtime.TotalHours));
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //Edit
                    
                    await RepoSection.UpdateSection(section, (int)id);
                    Section Section = await RepoSection.Sections
                        .Include(s => s.Trainer)
                        .Include(s => s.Trainees)
                        .Include(s => s.SectionField)
                        .FirstOrDefaultAsync(s => s.ID == id);
                    RepoTrainee.RemoveTraineeFromSection(Section);
                    if (selectedTrainees != null)
                    {
                        foreach (var trainee in selectedTrainees)
                        {
                            RepoTrainee.AddTraineeToSection(trainee, (int)id);
                        }
                    }
                    RepoSection.SendEvaluationEmail(Section);
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionID == null), "ID", "Name");
            ViewData["CreateOrEdit"] = "Edit Section";
            if (id == null)
            {
                ViewData["CreateOrEdit"] = "Add new Section";
            }
            return View();
        }
        public IActionResult CreateSectionField()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSectionField(SectionLookup model)
        {
            //if (sectionfield == null)
            //{
            //    return View();
            //}
            if (ModelState.IsValid)
            {
                await RepoSectionLookup.CreateSectionField(model.SectionField,model.Year);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult EditSectionField(int id)
        {
            SectionLookup SectionField = RepoSectionLookup.SectionLookUp.FirstOrDefault(s=>s.SectionLookupID==id);
            return View(SectionField);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSectionField(int id,SectionLookup model)
        {
            Console.WriteLine(id);
            Console.WriteLine(model.SectionField);
            Console.WriteLine(model.Year);
            if (ModelState.IsValid)
            {
                await RepoSectionLookup.EditSectionField(id,model.SectionField, model.Year);
                return RedirectToAction(nameof(Index));
            }
            SectionLookup SectionField = RepoSectionLookup.SectionLookUp.FirstOrDefault(s => s.SectionLookupID == id);
            return View(SectionField);
        }

        public IActionResult ViewTrainees(int id)
        {
            var section = RepoSection.Sections
                .Include(s => s.Trainer)
                    .Include(s => s.Trainees)
                    .Include(s => s.SectionField).FirstOrDefault(s => s.ID == id);
            PopulateTraineeDataForView(section);
            Trainer trainer = RepoTrainer.Trainers.FirstOrDefault(s => s.ID == section.TrainerID);
            section.Trainer = trainer;
            return View(section);
        }
        private void PopulateTraineeDataForView(Section section)
        {
            var allTrainee = RepoTrainee.Trainees;
            var Traineeinsection = section.Trainees.Where(S => S.SectionID == section.ID).Select(s => s.ID);
            var viewModel = new List<TraineesInSection>();
            foreach (var trainee in allTrainee)
            {
                if (Traineeinsection.Contains(trainee.ID))
                {
                    viewModel.Add(new TraineesInSection
                    {
                        TraineeID = trainee.ID,
                        Name = trainee.Name,
                        Email = trainee.Email,
                        Status = trainee.GraduationStatus,
                        Assigned = Traineeinsection.Contains(trainee.ID),
                        IsInOtherSection = trainee.SectionID != null
                    });
                }
            }
            ViewData["Trainees"] = viewModel;
        }
        public IActionResult SectionsReportPDF()
        {
            IQueryable<Section> sections = RepoSection.Sections
        .Include(s => s.Trainer)
        .Include(s => s.Trainees)
        .Include(s => s.SectionField)
        .Include(s => s.ProgramSection)
        .ThenInclude(s => s.programs);
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("StartDate");
            dt.Columns.Add("EndDate");
            dt.Columns.Add("Trainer");
            foreach (var section in sections)
            {
                Trainer trainer = RepoTrainer.Trainers.FirstOrDefault(s => s.ID == section.TrainerID);
                section.Trainer = trainer;
                if (section.ProgramSection == null)
                {

                    dt.Rows.Add(section.ID, section.SectionField.SectionField, section.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.Trainer.Name);

                }
                else
                {
                    dt.Rows.Add(section.ID, section.SectionField.SectionField, section.ProgramSection.programs.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.ProgramSection.programs.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.Trainer.Name);

                }

            }


            //var path = "C:\\Users\\arebaei\\source\\repos\\TrainingSystem\\Reports\\Reports\\Report3.rdlc";
            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report3.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result = lr.Execute(RenderType.Pdf, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/pdf");

        }
        public IActionResult SectionsReportExcel()
        {
            IQueryable<Section> sections = RepoSection.Sections
        .Include(s => s.Trainer)
        .Include(s => s.Trainees)
        .Include(s => s.SectionField)
        .Include(s => s.ProgramSection)
        .ThenInclude(s => s.programs);
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("StartDate");
            dt.Columns.Add("EndDate");
            dt.Columns.Add("Trainer");
            foreach (var section in sections)
            {
                Trainer trainer = RepoTrainer.Trainers.FirstOrDefault(s => s.ID == section.TrainerID);
                section.Trainer = trainer;
                if (section.ProgramSection == null)
                {
                    
                    dt.Rows.Add(section.ID, section.SectionField.SectionField, section.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.Trainer.Name);

                }
                else
                {
                    dt.Rows.Add(section.ID, section.SectionField.SectionField, section.ProgramSection.programs.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.ProgramSection.programs.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), section.Trainer.Name);

                }

            }


            //var path = "C:\\Users\\arebaei\\source\\repos\\TrainingSystem\\Reports\\Reports\\Report3.rdlc";
            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report3.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            //var result = lr.Execute(RenderType.Pdf, 1, parameters, "");

            //return new FileContentResult(result.MainStream, "application/pdf");
            var result = lr.Execute(RenderType.Excel, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/vnd.ms-excel");
        }
        }
}
