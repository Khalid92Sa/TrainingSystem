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
//using SelectPdf;

namespace TrainingSystem.Web.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITrainerService _TrainerService;
        private readonly ISection _section;

        public TrainersController(ITrainerService trainerService, ApplicationDbContext context, ISection section)
        {
            _TrainerService = trainerService;
            _context = context;
            _section = section;

        }

        // GET: Trainersf
        public async Task<IActionResult> Index(string SearchById, string SearchByName, string SearchByActivation, string SearchBySectionFeild)
        {
            if (string.IsNullOrEmpty(SearchById) && string.IsNullOrEmpty(SearchByName) && string.IsNullOrEmpty(SearchByActivation))
                return View( await _TrainerService.Trainers.ToListAsync());
            ViewBag.SearchById = SearchById;
            ViewBag.SearchByNmae = SearchByName;
            if (!string.IsNullOrEmpty(SearchByName)) return View(await _TrainerService.Trainers.Where(n => n.Name.Contains(SearchByName)).ToListAsync());
            else if (!string.IsNullOrEmpty(SearchBySectionFeild)) return View(await _TrainerService.Trainers.Where(s=> s.Section.SectionField.Contains(SearchBySectionFeild)).ToListAsync());
            else if (!string.IsNullOrEmpty(SearchById)) return View(await _TrainerService.Trainers.Where(n => n.ID.Contains(SearchById)).ToListAsync());
            else if (!string.IsNullOrEmpty(SearchByActivation))
            {
                if (SearchByActivation.Contains("Active")) return View(await _TrainerService.Trainers.Where(s => s.Status == true).ToListAsync());
                else return View(await _TrainerService.Trainers.Where(s => s.Status == false).ToListAsync());
            }
            if (string.IsNullOrEmpty(SearchValue)) return View( await _TrainerService.Trainers.ToListAsync());
            ViewBag.SearchValue = SearchValue;
            if (SearchBy == null || SearchBy == "Name") return View(await _TrainerService.Trainers.Where(n => n.Name.Contains(SearchValue)).ToListAsync());
            else if (SearchBy == "ID") return View(await _TrainerService.Trainers.Where(n => n.ID.Contains(SearchValue)).ToListAsync());
            else
            {
                return View(await _TrainerService.Trainers.ToListAsync());
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
