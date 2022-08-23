using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Users;
using TrainingSystem.Domain;
using TrainingSystem.Service;
using TrainingSystem.Web.Models;

namespace TrainingSystem.Web.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _UserService;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly ITrainerService _trainer;
        public HomeController(ILogger<HomeController> logger,
            IUserService userService,
            UserManager<IdentityUser> userManager,
            ITrainerService trainer
            )
        {
            _logger = logger;
            _UserService = userService;
            _UserManager = userManager;
            _trainer = trainer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //Login Page implementation

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _UserService.Passwordsignin(loginDTO);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Section");
                }
                ModelState.AddModelError(string.Empty, "invalid Login Attempt");
            }

            return View(loginDTO);
        }
        [HttpPost]
        [Route("Home/Login/{TraineeID}")]
        public  IActionResult Login(LoginDTO loginDTO,int TraineeID)
        {
            if (ModelState.IsValid)
            {
                var result = _trainer.Login(loginDTO.UserName, loginDTO.Password);
                if (result)
                {
                    return RedirectToAction("CreateEdit", new RouteValueDictionary(
                    new { controller = "Trainee", action = "CreateEdit", Id = TraineeID }));
                }
                ModelState.AddModelError(string.Empty, "invalid Login Attempt");
            }

            return View(loginDTO);
        }

        //Forgot Password Page implementation

        [HttpGet]
        public IActionResult ForgotPassword()
        {

            return View();
        }

        [HttpPost]
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

        [HttpGet]
        public IActionResult ResetPassword(string userName,string token)
        {
            ViewBag.userName = userName;
            ViewBag.token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (ModelState.IsValid)
            {
                var user =await _UserService.GetUserByUserName(resetPasswordDTO.UserName);
                if (user != null) {
                    var result = await _UserManager.ResetPasswordAsync(user,resetPasswordDTO.code,resetPasswordDTO.Password);
                    if (result.Succeeded)
                    {
                         return RedirectToAction("Login", "Home");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty,error.Description);
                    }
                    return View(resetPasswordDTO);
                }
                return RedirectToAction("Login", "Home");
            }
            return View(resetPasswordDTO);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
