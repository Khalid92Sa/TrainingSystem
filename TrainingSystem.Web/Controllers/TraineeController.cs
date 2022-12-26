using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service;
using AspNetCore.Reporting;
using TrainingSystem.Service.Interfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace TrainingSystem.Web.Controllers
{
    [Authorize]
    public class TraineeController : Controller
    {
        private readonly ITrainee RepoTrainee;
        private readonly ISection RepoSection;
        private readonly ISectionLookup RepoSectionLookup;
        private readonly ITrainerService RepoTrainer;
        private readonly IConfiguration _configuration;

        public TraineeController(
            ITrainee repoTrainee,
            ISection repoSection,
            ISectionLookup repoSectionLookup,
            ITrainerService repoTrainer,
            IConfiguration configuration
            )
        {
            RepoTrainee = repoTrainee;
            RepoSection = repoSection;
            RepoTrainer = repoTrainer;
            RepoSectionLookup = repoSectionLookup;
            _configuration= configuration;
        }

        public IActionResult Index(string SearchStringByID, string SearchStringByName, string Status, string Section)
        {
            ViewData["SearchByID"] = SearchStringByID;
            ViewData["SearchByName"] = SearchStringByName;
            ViewData["Status"] = Status;
            ViewData["Section"] = Section;
            ViewData["SectionField"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            IQueryable<Trainee> Trainees = RepoTrainee.Trainees
                    .Include(s => s.Section).ThenInclude(s => s.SectionField)
                    .Include(s => s.Section).ThenInclude(s => s.Trainer)
                    .Include(s => s.Trainer);
            if (SearchStringByID != null)
            {
                //Trainees = Trainees.Where(s => s.ID.ToString().Contains(SearchStringByID));
                Trainees = Trainees.Where(s => s.ID.ToString().Contains(SearchStringByID));
            }
            if (SearchStringByName != null)
            {
                Trainees = Trainees.Where(s => s.Name.Contains(SearchStringByName));
            }
            if (Status != null)
            {
                if (Status == "Active")
                {
                    Trainees = Trainees.Where(s => s.GraduationDate < DateTime.Now);
                }
                else
                {
                    Trainees = Trainees.Where(s => s.GraduationDate > DateTime.Now);
                }

            }
            if (Section != null)
            {
                Trainees = Trainees.Where(s => s.SectionLookupID.ToString().Contains(Section));
                SectionLookup sectionlookup = RepoSectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(Section));
                ViewData["SectionFieldResult"] = sectionlookup.SectionField;
            }
            return View(Trainees);
        }

        public ActionResult AddnewTrainee(int? SectionLookupID)
        {
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["SectionID"] = new SelectList(RepoSection.Sections, "ID", "SectionField");
            if(SectionLookupID != null)
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s=>(s.SectionLookupID==SectionLookupID || s.SectionLookupID1==SectionLookupID) && s.Status==true), "ID", "Name");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddnewTrainee(VMCETrainee model)
        public async Task<ActionResult> AddnewTrainee(VMCETrainee model, int? SectionLookupID)
        {
            ViewData["ErrorMessage"] = null ;
            if (ModelState.IsValid && RepoTrainee.RepetedName(model.Name))
            {
                //get file extension

                Trainee trainee = new Trainee();
                if (model.File != null)
                {
                    FileInfo fileInfo = new FileInfo(model.File.FileName);
                    string fileName = model.Name+"CV" + fileInfo.Extension;
                    using (var ms = new MemoryStream())
                    {
                        model.File.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);

                        trainee.Name = model.Name;
                        trainee.SectionLookupID = model.SectionLookupID;
                        trainee.TrainerID = model.TrainerID;
                        trainee.ContactNumber = model.ContactNumber;
                        trainee.StartDate = model.StartDate;
                        trainee.Email = model.Email;
                        trainee.GraduationDate = model.GraduationDate;
                        trainee.CV = fileBytes;
                        trainee.CVFileName = fileName;
                        trainee.HRFeedback = model.HRFeedback;
                    }
                }
                else
                {
                    trainee.Name = model.Name;
                    trainee.SectionLookupID = model.SectionLookupID;
                    trainee.TrainerID = model.TrainerID;
                    trainee.ContactNumber = model.ContactNumber;
                    trainee.StartDate = model.StartDate;
                    trainee.Email = model.Email;
                    trainee.GraduationDate = model.GraduationDate;
                    trainee.HRFeedback = model.HRFeedback;
                }
                await RepoTrainee.CreateTrainee(trainee);
                return RedirectToAction(nameof(Index));
            }
            if (!RepoTrainee.RepetedName(model.Name))
            {
                ViewData["ErrorMessage"] = "Name is already exist";
            }
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["SectionID"] = new SelectList(RepoSection.Sections, "ID", "SectionField");
            if (SectionLookupID != null)
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionLookupID == SectionLookupID || s.SectionLookupID1 == SectionLookupID), "ID", "Name");
            }
            
            return View();
        }
        public ActionResult CreateEdit(int? id, int? SectionLookupID)
        {
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["SectionID"] = new SelectList(RepoSection.Sections, "ID", "SectionField");

            ViewData["CreateOrEdit"] = "Edit Trainee";
            if (id == null)
            {
                ViewData["CreateOrEdit"] = "Add new Trainee";
                return View();
            }
            var Trainee = RepoTrainee.GetTraineeByID((int)id);
            if (SectionLookupID != null)
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => (s.SectionLookupID == SectionLookupID || s.SectionLookupID1 == SectionLookupID) && s.Status == true), "ID", "Name");
            }
            else
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => (s.SectionLookupID == Trainee.SectionLookupID || s.SectionLookupID1 == Trainee.SectionLookupID) && s.Status == true), "ID", "Name");
            }
            return View(Trainee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEdit([Bind("Name,Email,TrainerID,GraduationDate,HRFeedback,ContactNumber,SectionLookupID,CVFileName")] Trainee trainee, int id, int? SectionLookupID)
        {
            
            if (ModelState.IsValid &&RepoTrainee.RepetedNameupdate(trainee.Name,id))
            {
                trainee.StartDate = DateTime.Now;
                    await RepoTrainee.UpdateTrainee(trainee, (int)id);
                    return RedirectToAction(nameof(Index));
            }
            ViewData["CreateOrEdit"] = "Edit Trainee";
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers, "ID", "Name");
            if (!RepoTrainee.RepetedNameupdate(trainee.Name, id))
            {
                ViewData["ErrorMessage"] = "Name is already exist";
            }
            else
            {
                ViewData["ErrorMessage"] = null;
            }
            var Trainee = RepoTrainee.GetTraineeByID((int)id);
            if (SectionLookupID > 0)
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionLookupID == SectionLookupID || s.SectionLookupID1 == SectionLookupID), "ID", "Name");
            }
            else
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionLookupID == SectionLookupID || s.SectionLookupID1 == SectionLookupID || s.SectionLookupID == Trainee.SectionLookupID), "ID", "Name");
            }
            return View(Trainee);
        }
        public IActionResult AddCVFile(int id)
        {
            ViewData["ID"] = id;
            var trainee = RepoTrainee.GetTraineeByID(id);
            ViewData["Name"] = trainee.Name;
            if (trainee.CV == null)
            {
                ViewData["CreateOrEdit"] = "Create";
            }
            else
            {
                ViewData["CreateOrEdit"] = "Edit";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCVFile(VMUploadCVFile model)
        {
            if (ModelState.IsValid)
            {
                FileInfo fileInfo = new FileInfo(model.File.FileName);
                string fileName = model.FileName + fileInfo.Extension;
                if (model.File.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        model.File.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        await RepoTrainee.UpdateTraineeCV(fileName, fileBytes, model.ID);
                        // act on the Base64 data
                    }
                }
                return RedirectToAction("CreateEdit", new RouteValueDictionary(
                    new { controller = "Trainee", action = "CreateEdit", Id = model.ID }));
            }
            ViewData["ID"] = model.ID;
            return View();
        }
        public IActionResult ViewCVFile(int id)
        {
            ViewData["ID"] = id;
            var Trainee = RepoTrainee.GetTraineeByID(id);
            return View(Trainee);
        }
        public IActionResult TraineesReportPDF()
        {
            IQueryable<Trainee> Trainees = RepoTrainee.Trainees
                   .Include(s => s.SectionField)
                   .Include(s => s.Section).ThenInclude(s => s.Trainer)
                   .Include(s => s.Trainer);
            DataTable dataTable = new DataTable();
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("Trainer");
            dt.Columns.Add("GraduationStatus");

            foreach (var trainee in Trainees)
            {
                if (trainee.GraduationStatus)
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.SectionField.SectionField, trainee.Trainer.Name, "Active");
                }
                else
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.SectionField.SectionField, trainee.Trainer.Name, "Inactive");
                }
                
            }

            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report1.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var result = lr.Execute(RenderType.Pdf, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/pdf");

        }
        public IActionResult TraineesReportExcel()
        {


            IQueryable<Trainee> Trainees = RepoTrainee.Trainees
                   .Include(s => s.SectionField)
                   .Include(s => s.Section).ThenInclude(s => s.Trainer)
                   .Include(s => s.Trainer);
            DataTable dataTable = new DataTable();
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("Trainer");
            dt.Columns.Add("GraduationStatus");

            foreach (var trainee in Trainees)
            {
                if (trainee.GraduationStatus)
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.SectionField.SectionField, trainee.Trainer.Name, "Active");
                }
                else
                {
                    dt.Rows.Add(trainee.ID, trainee.Name, trainee.SectionField.SectionField, trainee.Trainer.Name, "Inactive");
                }

            }
            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report1.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result = lr.Execute(RenderType.Excel, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/vnd.ms-excel");


        }
    }
}
