﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service;
using TrainingSystem.Service.Interfaces;

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
                //string query = "SELECT DISTINCT  SectionLookupID as SectionField,MIN(StartDate) FROM Sections Group by SectionLookupID";
                string query = "SELECT SectionLookup.SectionField , MIN(Sections.StartDate) FROM Sections INNER JOIN SectionLookup ON Sections.SectionLookupID = SectionLookup.SectionLookupID WHERE Sections.SectionLookupID = SectionLookup.SectionLookupID GROUP BY SectionLookup.SectionField; ";
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
        public ActionResult CreateEdit(int? id)
        {
            ViewData["SectionLookupID"] = new SelectList(RepoSectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers, "ID", "Name");
            ViewData["CreateOrEdit"] = "Edit";
            if (id == null)
            {
                ViewData["CreateOrEdit"] = "Create";
                return View();
            }

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
            ViewData["TrainerID"] = new SelectList(RepoTrainer.Trainers, "ID", "Name");
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
                    IsInOtherSection= trainee.SectionID != null
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

    }
}
