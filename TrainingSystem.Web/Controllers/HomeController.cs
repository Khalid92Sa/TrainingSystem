using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Users;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service;
using TrainingSystem.Service.Interfaces;
using TrainingSystem.Web.Models;

namespace TrainingSystem.Web.Controllers
{


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _UserService;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly ITrainerService _trainer;
        private readonly ITrainee _trainee;
        private readonly ISection _section;
        private readonly ISectionLookup _sectionLookup;
        private readonly IprogramsService _program;
        private readonly SignInManager<IdentityUser> _signInManager;
        public HomeController(ILogger<HomeController> logger,
            IUserService userService,
            UserManager<IdentityUser> userManager,
            ITrainerService trainer,
            ITrainee trainee,
            ISection section,
            ISectionLookup sectionLookup,
            IprogramsService program,
            SignInManager<IdentityUser> signInManager
            )
        {
            _logger = logger;
            _UserService = userService;
            _UserManager = userManager;
            _trainer = trainer;
            _trainee = trainee;
            _section = section;
            _sectionLookup = sectionLookup;
            _program = program;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Welcome()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _UserService.Passwordsignin(loginDTO);
                if (result.Succeeded)
                {

                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Welcome", "Home");
                }
                ModelState.AddModelError(string.Empty, "invalid Login Attempt");
            }

            return View(loginDTO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        [HttpPost]
        [Route("Home/Index/{SectionID}")]
        [AllowAnonymous]
        public IActionResult Index(LoginDTO loginDTO, int SectionID)
        {
            if (ModelState.IsValid)
            {
                var result = _trainer.Login(loginDTO.UserName, loginDTO.Password, SectionID);
                if (result)
                {
                    return RedirectToAction("InsertTrainees", new RouteValueDictionary(
                    new { controller = "Section", action = "InsertTrainees", Id = SectionID }));
                }
                ModelState.AddModelError(string.Empty, "invalid Login Attempt");
            }

            return View(loginDTO);
        }

        //Forgot Password Page implementation
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ResponseDTO> ForgotPassword(ForgotPasswordDTo forgotPasswordDTo)
        {
            if (ModelState.IsValid)
            {
                var result = await _UserService.ForgotPassword(forgotPasswordDTo);
                return result;
            }
            return null;

        }

        public async Task<ConfrimationCodeResponseDTO> ConfirmationCode(string userName, int code)
        {
            var result = _UserService.ConfirmationCode(userName, code);
            var res = new ConfrimationCodeResponseDTO();
            result = true;
            if (result)
            {
                var user = await _UserService.GetUserByUserName(userName);
                var token = await _UserManager.GeneratePasswordResetTokenAsync(user);
                res.token = token;

            }
            else
            {
                res.message = "please try again";
            }
            res.IsSuccess = result;

            return res;
        }

        //Reset Password Page implementation
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string userName, string token)
        {
            ViewBag.userName = userName;
            ViewBag.token = token;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _UserService.GetUserByUserName(resetPasswordDTO.UserName);
                if (user != null)
                {
                    var result = await _UserManager.ResetPasswordAsync(user, resetPasswordDTO.code, resetPasswordDTO.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", "Home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(resetPasswordDTO);
                }
                return RedirectToAction("Login", "Home");
            }
            return View(resetPasswordDTO);
        }
        [Authorize]
        public IActionResult ReportingPage(string Trainer, string Trainee, string Section, string Status, string Score)
        {
            ViewData["Trainer"] = Trainer;
            ViewData["Trainee"] = Trainee;
            ViewData["Section"] = Section;
            ViewData["Status"] = Status;
            ViewData["Score"] = Score;
            ViewData["Trainers"] = new SelectList(_trainer.Trainers, "ID", "Name");
            ViewData["SectionField"] = new SelectList(_sectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
            ViewData["Trainees"] = new SelectList(_trainee.Trainees, "ID", "Name");
            IQueryable<Trainee> Trainees = _trainee.Trainees
                .Include(s => s.Section)
                    .ThenInclude(s => s.Trainer)
                .Include(s => s.Section)
                    .ThenInclude(s => s.SectionField);
            //.Include(s => s.Section)
            //    .ThenInclude(s => s);

            if (Trainer != null)
            {
                if (Trainer == "NotIncludeSection")
                {
                    Trainees = Trainees.Where(s => s.SectionID == null);
                    ViewData["TrainerResult"] = "Not Include Section";
                }
                else
                {
                    Trainees = Trainees.Where(s => s.Section.Trainer.ID.Contains(Trainer));
                    Trainer trainer = _trainer.Trainers.First(s => s.ID == Trainer);
                    ViewData["TrainerResult"] = trainer.Name;
                }
            }
            if (Trainee != null)
            {
                Trainees = Trainees.Where(s => s.ID.ToString().Contains(Trainee));
                Trainee trainee = _trainee.Trainees.First(s => s.ID.ToString().Contains(Trainee));
                ViewData["TraineeResult"] = trainee.Name;
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
                    Trainees = Trainees.Where(s => s.Section.SectionField.SectionLookupID.ToString().Contains(Section));
                    SectionLookup sectionlookup = _sectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(Section));
                    ViewData["SectionFieldResult"] = sectionlookup.SectionField;
                }

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
            //if (Score != null)
            //{
            //    Trainees = Trainees.Where();
            //}
            IQueryable<Programs> program = _program.Programs
                .Include(s => s.programSections);
            var Data = new List<ViewModelReporting> { };
            var EvaluationScore = 110;
            bool z;
            foreach (var item in Trainees)
            {

                EvaluationScore = EvaluationScore - 10;
                if (item.SectionID == null)
                {
                    Data.Add(new ViewModelReporting
                    {
                        Trainee = item.Name,
                        IsInSection = false
                    });
                }
                else
                {
                    foreach (var x in program)
                    {
                        z = x.programSections.Where(s => s.SectionID == item.SectionID).Any();

                    }
                    //z = program.programSections.Where(s => s.SectionID == item.SectionID).Any();
                    Data.Add(new ViewModelReporting
                    {
                        Trainee = item.Name,
                        Trainer = item.Section.Trainer.Name,
                        Sectionfield = item.Section.SectionField.SectionField,
                        Status = item.GraduationStatus,
                        Evaluation = EvaluationScore,
                        IsInSection = true
                    });
                }

            }
            return View(Data);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
