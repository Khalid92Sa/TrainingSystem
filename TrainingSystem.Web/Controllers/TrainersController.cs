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
//using SelectPdf;

namespace TrainingSystem.Web.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITrainerService _TrainerService;
        private readonly ISectionLookup _SectionLookup;
        private readonly ISection _section;

        public TrainersController(ITrainerService trainerService,
            ApplicationDbContext context,
            ISection section,
            ISectionLookup sectionLookup)
        {
            _TrainerService = trainerService;
            _context = context;
            _section = section;
            _SectionLookup = sectionLookup;

        }

        // GET: Trainersf
        public async Task<IActionResult> Index(string SearchById, string SearchByName, string SearchByActivation, string SearchBySectionFeild)
        {
            ViewData["SectionField"] = new SelectList(_SectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["searchSection"] = SearchBySectionFeild;
            if (string.IsNullOrEmpty(SearchById) && string.IsNullOrEmpty(SearchByName) && string.IsNullOrEmpty(SearchByActivation) && string.IsNullOrEmpty(SearchBySectionFeild))
                return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).ToListAsync());
            ViewBag.SearchById = SearchById;
            ViewBag.SearchByNmae = SearchByName;
            if (!string.IsNullOrEmpty(SearchByName)) return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).Where(n => n.Name.Contains(SearchByName)).ToListAsync());
            else if (!string.IsNullOrEmpty(SearchBySectionFeild))
            {
                if (SearchBySectionFeild == "DoesnottrainanySection")
                {
                    ViewData["SectionFieldResult"] = "Does not train any Section";
                    return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).Where(s => s.SectionID == null).ToListAsync());

                }
                else
                {
                    SectionLookup sectionlookup = _SectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(SearchBySectionFeild));
                    ViewData["SectionFieldResult"] = sectionlookup.SectionField;
                    return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).Where(s => s.Section.SectionField.SectionLookupID.ToString().Contains(SearchBySectionFeild)).ToListAsync());

                }
            }
            else if (!string.IsNullOrEmpty(SearchById)) return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).Where(n => n.ID.Contains(SearchById)).ToListAsync());
            else if (!string.IsNullOrEmpty(SearchByActivation))
            {
                if (SearchByActivation.Contains("Active")) return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).Where(s => s.Status == true).ToListAsync());
                else return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).Where(s => s.Status == false).ToListAsync());
            }
            else
            {
                return View(await _TrainerService.Trainers.Include(s => s.Section).ThenInclude(s => s.SectionField).ToListAsync());
            }
            //return View(result);
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(string id)
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
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,SectionID,Status,ContactNumber,Address,UserName,Email,Password")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                 _TrainerService.AddTrainer(trainer);
                await _TrainerService.SaveChangesAsyncc();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(string id)
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
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Name,SectionID,Status,ContactNumber,Address,UserName,Email,Password")] Trainer trainer)
        {
            if (id != trainer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _TrainerService.UpdateTrainer(trainer);
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
            ViewData["SectionID"] = new SelectList(_section.Sections, "ID", "SectionField");
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        
        public async Task<IActionResult> Delete(string id)
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
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

        private bool TrainerExists(string id)
        {
            return _context.Trainers.Any(e => e.ID == id);
        }
    }
}
