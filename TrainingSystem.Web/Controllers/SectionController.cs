using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service;
using TrainingSystem.Service.Interfaces;
using Xamarin.Essentials;

namespace TrainingSystem.Web.Controllers
{
    public class SectionController : Controller
    {
        private readonly ITrainee RepoTrainee;
        private readonly ISection RepoSection;
        private readonly ISectionLookup RepoSectionLookup;
        private readonly ITrainerService RepoTrainer;
        public SectionController(
            ITrainee repoTrainee,
            ISection repoSection,
            ISectionLookup repoSectionLookup,
            ITrainerService repoTrainer
            )
        {
            RepoTrainee = repoTrainee;
            RepoSection = repoSection;
            RepoSectionLookup = repoSectionLookup;
            RepoTrainer = repoTrainer;
        }
        public async Task<ActionResult> Index(string searchSection, string DateFilter, string DateFilterFields)
        {
            ViewData["CurrentFilterDateFields"] = DateFilterFields;
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["searchSection"] = searchSection;
            ViewData["CurrentFilterDate"] = DateFilter;
            ViewData["SectionField"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            IQueryable<Section> sections = RepoSection.Sections
                    .Include(s => s.Trainer)
                    .Include(s => s.Trainees)
                    .Include(s => s.SectionField);
            if (searchSection != null)
            {
                sections = sections
                    .Where(s => s.SectionField.SectionLookupID.ToString().Contains(searchSection));
                SectionLookup sectionlookup = RepoSectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(searchSection));
                ViewData["SectionFieldResult"] = sectionlookup.SectionField;
            }
            if (DateFilter != null)
            {
                sections = sections
                    .Where(s => s.StartDate.ToString().Contains(DateFilter));
            }
            List<SectionsFields> groups = new List<SectionsFields>();
            var conn = RepoSection.Conn();
            try
            {
                await conn.OpenAsync();
                using var command = conn.CreateCommand();
                //string query = "SELECT SectionLookup.SectionField , MIN(Sections.StartDate) FROM Sections INNER JOIN SectionLookup ON Sections.SectionLookupID = SectionLookup.SectionLookupID WHERE Sections.SectionLookupID = SectionLookup.SectionLookupID GROUP BY SectionLookup.SectionField; ";
                string query = String.Join(
                    Environment.NewLine,
                    "SELECT SectionLookup.SectionField , MIN(Sections.StartDate)",
                    "FROM Sections INNER JOIN SectionLookup",
                    "ON Sections.SectionLookupID = SectionLookup.SectionLookupID",
                    "WHERE Sections.SectionLookupID = SectionLookup.SectionLookupID",
                    "GROUP BY SectionLookup.SectionField;");
                command.CommandText = query;
                DbDataReader reader = await command.ExecuteReaderAsync();
                var id = 1;
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new SectionsFields { ID = id++, SectionField = reader.GetString(0), Year = reader.GetDateTime(1) };
                        groups.Add(row);
                    }
                }
                reader.Dispose();
            }
            finally
            {
                conn.Close();
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
        [HttpPost]
        public async Task<IActionResult> Evaluation(int SectionID)
        {
            Section Section = await RepoSection.Sections
                .Include(s => s.Trainer)
                .Include(s => s.Trainees)
                .Include(s => s.SectionField)
                .FirstOrDefaultAsync(s => s.ID == SectionID);
            string trainees = @"";
            int count = 1;
            foreach (var x in Section.Trainees)
            {
                trainees += count + ")" + x.Name +"\n";
                count++;
            }
            try
            {
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                service.UseDefaultCredentials = false;
                service.Credentials = new WebCredentials("abdulkareem.rabbai@techprocess.net", "P@ssw0rd", "sss-process.org");
                service.Url = new Uri("https://mail.sssprocess.com/EWS/Exchange.asmx");
                Microsoft.Exchange.WebServices.Data.EmailMessage emailMessage = new Microsoft.Exchange.WebServices.Data.EmailMessage(service);
                emailMessage.Subject = "Mr.\\Mrs. " + Section.Trainer.Name + ",";
                emailMessage.Body = String.Join(
                    Environment.NewLine,
                    "Evalution of the Trainees:",
                    trainees,
                    "Please Evalute Trainees By The Link: " + "https://localhost:44321/Home/Login/" + SectionID,
                    "Regards,");
                emailMessage.Body.BodyType = BodyType.HTML;
                emailMessage.ToRecipients.Add(Section.Trainer.Email);
               // emailMessage.Send();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailToTrainerToStart(int SectionID)
        {
            Section Section = await RepoSection.Sections
                .Include(s => s.Trainer)
                .Include(s => s.Trainees)
                .Include(s => s.SectionField)
                .FirstOrDefaultAsync(s => s.ID == SectionID);
            string trainees = @"";
            int count = 1;
            foreach (var x in Section.Trainees)
            {
                trainees +=count+")"+ x.Name + " Email:"+x.Email+"\n";
                count++;
            }
            try
            {
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
                service.UseDefaultCredentials = false;
                service.Credentials = new WebCredentials("abdulkareem.rabbai@techprocess.net", "P@ssw0rd", "sss-process.org");
                service.Url = new Uri("https://mail.sssprocess.com/EWS/Exchange.asmx");
                Microsoft.Exchange.WebServices.Data.EmailMessage emailMessage = new Microsoft.Exchange.WebServices.Data.EmailMessage(service);
                emailMessage.Subject = "Mr.\\Mrs. " + Section.Trainer.Name + ",";
                emailMessage.Body = String.Join(
                    Environment.NewLine,
                    "Our company has opened a new training track.",
                    "You have been handed the " + Section.SectionField.SectionField + " Track with each of the following interns:",
                     trainees,
                    "Regards,");
                emailMessage.Body.BodyType = BodyType.HTML;
                emailMessage.ToRecipients.Add(Section.Trainer.Email);
                // emailMessage.Send();
            }
            catch (Exception)
            {
                throw;
            }
            
            return RedirectToAction(nameof(Index));
        }
        public ActionResult CreateEdit(int? id)
        {
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["CreateOrEdit"] = "Edit";
            if (id == null)
            {
                ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionID == null), "ID", "Name");
                ViewData["CreateOrEdit"] = "Create";
                return View();
            }
            ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionID == null || s.SectionID == id), "ID", "Name");
            var section = RepoSection.GetSectionByID((int)id);
            return View(section);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEdit([Bind("SectionLookupID,StartDate,TrainerID")] Section section, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    //Create
                    await RepoSection.CreateSection(section);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //Edit
                    await RepoSection.UpdateSection(section, (int)id);
                    return RedirectToAction(nameof(Index));

                }
            }
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers.Where(s => s.SectionID == null), "ID", "Name");
            ViewData["CreateOrEdit"] = "Edit";
            if (id == null)
            {
                ViewData["CreateOrEdit"] = "Create";
            }
            return View();
        }
        public IActionResult InsertTrainees(int id)
        {
            var section = RepoSection.Sections
                .Include(s => s.Trainer)
                    .Include(s => s.Trainees)
                    .Include(s => s.SectionField).FirstOrDefault(s => s.ID == id);
            PopulateTraineeData(section);
            return View(section);
        }
        private void PopulateTraineeData(Section section)
        {
            var allTrainee = RepoTrainee.Trainees;
            var Traineeinsection = section.Trainees.Where(S => S.SectionID == section.ID).Select(s => s.ID);
            var viewModel = new List<TraineesInSection>();
            foreach (var trainee in allTrainee)
            {
                viewModel.Add(new TraineesInSection
                {
                    TraineeID = trainee.ID,
                    Name = trainee.Name,
                    Assigned = Traineeinsection.Contains(trainee.ID),
                    IsInOtherSection = trainee.SectionID != null
                });
            }
            ViewData["Trainees"] = viewModel;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertTrainees(int id, int[] selectedTrainees)
        {
            var section = RepoSection.GetSectionByID(id);
            RepoTrainee.RemoveTraineeFromSection(section);
            if (selectedTrainees != null)
            {
                foreach (var trainee in selectedTrainees)
                {
                    RepoTrainee.AddTraineeToSection(trainee, id);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ViewTrainees(int id)
        {
            var section = RepoSection.Sections
                .Include(s => s.Trainer)
                    .Include(s => s.Trainees)
                    .Include(s => s.SectionField).FirstOrDefault(s => s.ID == id);
            PopulateTraineeDataForView(section);
            return View(section);
        }
        private void PopulateTraineeDataForView(Section section)
        {
            var allTrainee = RepoTrainee.Trainees;
            var Traineeinsection = section.Trainees.Where(S => S.SectionID == section.ID).Select(s => s.ID);
            var viewModel = new List<TraineesInSection>();
            foreach (var trainee in allTrainee)
            {
                if(Traineeinsection.Contains(trainee.ID))
                {
                    viewModel.Add(new TraineesInSection
                    {
                        TraineeID = trainee.ID,
                        Name = trainee.Name,
                        Email = trainee.Email,
                        Status=trainee.GraduationStatus,
                        Assigned = Traineeinsection.Contains(trainee.ID),
                        IsInOtherSection = trainee.SectionID != null
                    });
                }
            }
            ViewData["Trainees"] = viewModel;
        }
    }
}
