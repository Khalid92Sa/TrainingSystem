using AspNetCore.Reporting;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger,
            IUserService userService,
            UserManager<IdentityUser> userManager,
            ITrainerService trainer,
            ITrainee trainee,
            ISection section,
            ISectionLookup sectionLookup,
            IprogramsService program,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration
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
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
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
                    return RedirectToAction("Index", "Trainers");
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
        [Route("Home/Index/{TrainerID}")]
        [AllowAnonymous]
        public IActionResult Index(LoginDTO loginDTO, int TrainerID)
        {
            _logger.LogInformation("trainerID:" + TrainerID);
            try
            {
               if (ModelState.IsValid)
            {
                var result = _trainer.Login(loginDTO.UserName, loginDTO.Password, TrainerID);
                if (result)
                {
                    BackgroundJob.Schedule(() => _trainer.LogoutTrainer(TrainerID), TimeSpan.FromMinutes(10));
                    return RedirectToAction("Index", new RouteValueDictionary(
                     new { controller = "Evaluation", action = "Index", Id = TrainerID }));
                }
                ModelState.AddModelError(string.Empty, "invalid Login Attempt");
            }

            return View(loginDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
           
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
        [AllowAnonymous]
        public async Task<ConfrimationCodeResponseDTO> ConfirmationCode(string userName, int code)
        {
            
            var result = _UserService.ConfirmationCode(userName, code);
            var res = new ConfrimationCodeResponseDTO();
            var user = await _UserService.GetUserByUserName(userName);
          
            
            if (result && user != null)
            {
                var token = await _UserManager.GeneratePasswordResetTokenAsync(user);
                res.token = token;
                res.message = "Success";
            }
            else if (result && user == null)
            {
                res.message = "TrainerSuccess";
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
                        return RedirectToAction("Index", "Home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    ViewData["token"] = resetPasswordDTO.code;
                    return View(resetPasswordDTO);
                }
                return RedirectToAction("Index", "Home");
            }
            ViewData["token"] = resetPasswordDTO.code;
            return View(resetPasswordDTO);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPasswordTrainer(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordTrainer(ResetPasswordDTO resetPasswordDTO)
        {
            if (ModelState.IsValid)
            {
                var trainer = _trainer.Trainers.FirstOrDefault(s=>s.UserName== resetPasswordDTO.UserName);
                if (trainer is null)
                {
                    return NotFound();
                }
                trainer.Password = resetPasswordDTO.Password;
                await _trainer.SaveChangesAsyncc();
                return RedirectToAction("Index", new RouteValueDictionary(
                    new { controller = "Evaluation", action = "Index", Id = trainer.ID }));
            }
            ViewBag.userName = resetPasswordDTO.UserName;
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
            
            if (Trainer != null)
            {
                Trainer trainer = _trainer.Trainers.Where(s => s.ID.ToString() == Trainer).First();
                ViewData["SectionField"] = new SelectList(_sectionLookup.SectionLookUp.Where(s => s.SectionLookupID == trainer.SectionLookupID1 || s.SectionLookupID == trainer.SectionLookupID), "SectionLookupID", "SectionField");
                ViewData["Trainees"] = new SelectList(_trainee.Trainees.Where(s=>s.TrainerID.ToString()==Trainer), "ID", "Name");
            }
            else
            {
                ViewData["SectionField"] = new SelectList(_sectionLookup.SectionLookUp, "SectionLookupID", "SectionField");
                ViewData["Trainees"] = new SelectList(_trainee.Trainees, "ID", "Name");
            }
            
            IQueryable<Trainee> Trainees = _trainee.Trainees
                .Include(s => s.SectionField)
                .Include(s => s.Section)
                    .ThenInclude(s => s.Trainer)
                .Include(s => s.Section)
                    .ThenInclude(s => s.SectionField)
                .Include(s => s.Evaluation)
                .Include(s => s.Trainer);
            if (Trainer != null)
            {
                if (Trainer == "NotIncludeSection")
                {
                    Trainees = Trainees.Where(s => s.SectionID == null);
                    ViewData["TrainerResult"] = "Not Include Section";
                }
                else
                {
                    Trainees = Trainees.Where(s => s.Section.Trainer.ID.ToString().Contains(Trainer));
                    Trainer trainer = _trainer.Trainers.First(s => s.ID.ToString() == Trainer);
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

                Trainees = Trainees.Where(s => s.SectionLookupID.ToString().Contains(Section));
                SectionLookup sectionlookup = _sectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(Section));
                ViewData["SectionFieldResult"] = sectionlookup.SectionField;


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
            if (Score != null)
            {

                if (Score == "NoEvaluation")
                {
                    Trainees = Trainees.Where(s => s.Evaluation == null);
                }
                else
                {
                    int num1 = 100, num2 = 0;
                    if (Score == "Excellent")
                    {
                        num1 = 100;
                        num2 = 90;
                    }
                    else if (Score == "Very Good")
                    {
                        num1 = 90;
                        num2 = 80;
                    }
                    else if (Score == "Good")
                    {
                        num1 = 80;
                        num2 = 70;
                    }
                    else
                    {
                        num1 = 70;
                        num2 = 0;
                    }
                    Trainees = Trainees.Where(s => s.Evaluation != null);
                    Trainees = Trainees.Where(s => s.Evaluation.EvaluationRate <= num1);
                    Trainees = Trainees.Where(s => s.Evaluation.EvaluationRate >= num2);
                }
            }
            var Data = new List<ViewModelReporting> { };
            foreach (var item in Trainees)
            {
                if (item.SectionID == null)
                {
                    Data.Add(new ViewModelReporting
                    {
                        Trainee = item.Name,
                        Trainer = item.Trainer.Name,
                        Status = item.GraduationStatus,
                        Sectionfield = item.SectionField.SectionField,
                        IsInSection = false
                    });
                }
                else
                {
                    if (item.Evaluation == null)
                    {
                        Data.Add(new ViewModelReporting
                        {
                            Trainee = item.Name,
                            Trainer = item.Trainer.Name,
                            Sectionfield = item.SectionField.SectionField,
                            Status = item.GraduationStatus,
                            Evaluation = 0,
                            IsInSection = true
                        });
                    }
                    else
                    {

                        Data.Add(new ViewModelReporting
                        {
                            Trainee = item.Name,
                            Trainer = item.Trainer.Name,
                            Sectionfield = item.SectionField.SectionField,
                            Status = item.GraduationStatus,
                            Evaluation = item.Evaluation.EvaluationRate,
                            IsInSection = true,
                            feedback = item.Evaluation.feedback
                        });


                    }

                }

            }
            return View(Data);
        }
        public IActionResult ReportingPDF(string Trainer, string Trainee, string Section, string Status, string Score)
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
                 .Include(s => s.SectionField)
                 .Include(s => s.Section)
                     .ThenInclude(s => s.Trainer)
                 .Include(s => s.Section)
                     .ThenInclude(s => s.SectionField)
                 .Include(s => s.Evaluation)
                 .Include(s => s.Trainer);
            if (Trainer != null)
            {
                if (Trainer == "NotIncludeSection")
                {
                    Trainees = Trainees.Where(s => s.SectionID == null);
                    ViewData["TrainerResult"] = "Not Include Section";
                }
                else
                {
                    Trainees = Trainees.Where(s => s.Section.Trainer.ID.ToString().Contains(Trainer));
                    Trainer trainer = _trainer.Trainers.First(s => s.ID.ToString() == Trainer);
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

                Trainees = Trainees.Where(s => s.SectionLookupID.ToString().Contains(Section));
                SectionLookup sectionlookup = _sectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(Section));
                ViewData["SectionFieldResult"] = sectionlookup.SectionField;


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
            if (Score != null)
            {

                if (Score == "NoEvaluation")
                {
                    Trainees = Trainees.Where(s => s.Evaluation == null);
                }
                else
                {
                    int num1 = 100, num2 = 0;
                    if (Score == "Excellent")
                    {
                        num1 = 100;
                        num2 = 90;
                    }
                    else if (Score == "Very Good")
                    {
                        num1 = 90;
                        num2 = 80;
                    }
                    else if (Score == "Good")
                    {
                        num1 = 80;
                        num2 = 70;
                    }
                    else
                    {
                        num1 = 70;
                        num2 = 0;
                    }
                    Trainees = Trainees.Where(s => s.Evaluation != null);
                    Trainees = Trainees.Where(s => s.Evaluation.EvaluationRate <= num1);
                    Trainees = Trainees.Where(s => s.Evaluation.EvaluationRate >= num2);
                }
            }
            DataTable dt = new DataTable();

            dt.Columns.Add("Trainee");
            dt.Columns.Add("Trainer");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("GraduationStatus");
            dt.Columns.Add("EvaluationScore");
            dt.Columns.Add("FeedBack");
            foreach (var item in Trainees)
            {
                if (item.SectionID == null)
                {
                    if (item.GraduationStatus)
                    {
                        dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Not Include Section", "Not Include Section");
                    }
                    else
                    {
                        dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Not Include Section", "Not Include Section");
                    }

                }
                else
                {
                    if (item.Evaluation == null)
                    {
                        if (item.GraduationStatus)
                        {
                            dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "No Evauation", "No Evaluation");
                        }
                        else
                        {
                            dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "No Evauation", "No Evaluation");
                        }

                    }
                    else
                    {
                        if (item.GraduationStatus)
                        {
                           
                            if(item.Evaluation.EvaluationRate <= 70)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Poor", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 79)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Good", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 89)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Very Good", item.Evaluation.feedback);
                            }
                            else
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Excellent", item.Evaluation.feedback);
                            }
                        }
                        else
                        {
                            if (item.Evaluation.EvaluationRate <= 70)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Poor", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 79)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Good", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 89)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Very Good", item.Evaluation.feedback);
                            }
                            else
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Excellent", item.Evaluation.feedback);
                            }
                        }

                    }
                }
            }

            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report5.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result = lr.Execute(RenderType.Pdf, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/pdf");

        }
        public IActionResult ReportingExcel(string Trainer, string Trainee, string Section, string Status, string Score)
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
                 .Include(s => s.SectionField)
                 .Include(s => s.Section)
                     .ThenInclude(s => s.Trainer)
                 .Include(s => s.Section)
                     .ThenInclude(s => s.SectionField)
                 .Include(s => s.Evaluation)
                 .Include(s => s.Trainer);
            if (Trainer != null)
            {
                if (Trainer == "NotIncludeSection")
                {
                    Trainees = Trainees.Where(s => s.SectionID == null);
                    ViewData["TrainerResult"] = "Not Include Section";
                }
                else
                {
                    Trainees = Trainees.Where(s => s.Section.Trainer.ID.ToString().Contains(Trainer));
                    Trainer trainer = _trainer.Trainers.First(s => s.ID.ToString() == Trainer);
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

                Trainees = Trainees.Where(s => s.SectionLookupID.ToString().Contains(Section));
                SectionLookup sectionlookup = _sectionLookup.SectionLookUp.First(s => s.SectionLookupID.ToString().Contains(Section));
                ViewData["SectionFieldResult"] = sectionlookup.SectionField;


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
            if (Score != null)
            {

                if (Score == "NoEvaluation")
                {
                    Trainees = Trainees.Where(s => s.Evaluation == null);
                }
                else
                {
                    int num1 = 100, num2 = 0;
                    if (Score == "Excellent")
                    {
                        num1 = 100;
                        num2 = 90;
                    }
                    else if (Score == "Very Good")
                    {
                        num1 = 90;
                        num2 = 80;
                    }
                    else if (Score == "Good")
                    {
                        num1 = 80;
                        num2 = 70;
                    }
                    else
                    {
                        num1 = 70;
                        num2 = 0;
                    }
                    Trainees = Trainees.Where(s => s.Evaluation != null);
                    Trainees = Trainees.Where(s => s.Evaluation.EvaluationRate <= num1);
                    Trainees = Trainees.Where(s => s.Evaluation.EvaluationRate >= num2);
                }
            }
            DataTable dt = new DataTable();

            dt.Columns.Add("Trainee");
            dt.Columns.Add("Trainer");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("GraduationStatus");
            dt.Columns.Add("EvaluationScore");
            dt.Columns.Add("FeedBack");
            foreach (var item in Trainees)
            {
                if (item.SectionID == null)
                {
                    if (item.GraduationStatus)
                    {
                        dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Not Include Section", "Not Include Section");
                    }
                    else
                    {
                        dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Not Include Section", "Not Include Section");
                    }

                }
                else
                {
                    if (item.Evaluation == null)
                    {
                        if (item.GraduationStatus)
                        {
                            dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "No Evauation", "No Evaluation");
                        }
                        else
                        {
                            dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "No Evauation", "No Evaluation");
                        }

                    }
                    else
                    {
                        if (item.GraduationStatus)
                        {

                            if (item.Evaluation.EvaluationRate <= 70)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Poor", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 79)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Good", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 89)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Very Good", item.Evaluation.feedback);
                            }
                            else
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Active", "Excellent", item.Evaluation.feedback);
                            }
                        }
                        else
                        {
                            if (item.Evaluation.EvaluationRate <= 70)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Poor", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 79)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Good", item.Evaluation.feedback);
                            }
                            else if (item.Evaluation.EvaluationRate <= 89)
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Very Good", item.Evaluation.feedback);
                            }
                            else
                            {
                                dt.Rows.Add(item.Name, item.Trainer.Name, item.SectionField.SectionField, "Inactive", "Excellent", item.Evaluation.feedback);
                            }
                        }

                    }
                }
            }

            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report5.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet1", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result = lr.Execute(RenderType.Excel, 1, parameters, "");

            return new FileContentResult(result.MainStream, "application/vnd.ms-excel");

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
