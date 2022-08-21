using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Trainees;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Web.Controllers
{
    public class TraineeController : Controller
    {
        private readonly ITrainee RepoTrainee;
        private readonly ISection RepoSection;
        private readonly ISectionLookup RepoSectionLookup;
        private readonly ITrainerService RepoTrainer;

        public TraineeController(
            ITrainee repoTrainee,
            ISection repoSection,
            ISectionLookup repoSectionLookup,
            ITrainerService repoTrainer
            )
        {
            RepoTrainee = repoTrainee;
            RepoSection = repoSection;
            RepoTrainer = repoTrainer;
            RepoSectionLookup = repoSectionLookup;
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
                if (Section == "NotIncludeSection")
                {
                    Trainees = Trainees.Where(s => s.SectionID == null);
                    ViewData["SectionFieldResult"] = "Not Include Section";
                }
                else
                {
                    Trainees = Trainees.Where(s => s.Section.SectionLookupID.ToString().Contains(Section));
                    SectionLookup sectionlookup = RepoSectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(Section));
                    ViewData["SectionFieldResult"] = sectionlookup.SectionField;
                }

            }
            return View(Trainees);
        }
        public ActionResult CreateEdit(int? id)
        {
            ViewData["SectionID"] = new SelectList(RepoSection.Sections, "ID", "SectionField");
            ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers, "ID", "Name");
            ViewData["CreateOrEdit"] = "Edit";
            if (id == null)
            {
                ViewData["CreateOrEdit"] = "Create";
                return View();
            }
            var Trainee = RepoTrainee.GetTraineeByID((int)id);
            return View(Trainee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEdit([Bind("Name,Email,GraduationDate,CVFileName,ContactNumber,StartDate")] Trainee trainee, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    //Create
                    await RepoTrainee.CreateTrainee(trainee);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //Edit
                    await RepoTrainee.UpdateTrainee(trainee, (int)id);
                    return RedirectToAction(nameof(Index));

                }
            }
            ViewData["CreateOrEdit"] = "Edit";
            if (id == null)
            {
                ViewData["CreateOrEdit"] = "Create";
            }
            return View();
        }
        public IActionResult AddCVFile(int id)
        {
            ViewData["ID"] = id;
            var trainee = RepoTrainee.GetTraineeByID(id);
            ViewData["Name"] = trainee.Name;
            if(trainee.CV==null)
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
                //get file extension
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["ID"] = model.ID;
            return View();
        }
        public IActionResult ViewCVFile(int id)
        {
            var Trainee = RepoTrainee.GetTraineeByID(id);
            return View(Trainee);
        }
    }
}
