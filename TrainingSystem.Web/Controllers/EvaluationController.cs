using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Evaluation;
using TrainingSystem.Domain;
using TrainingSystem.Service;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Web.Controllers
{
    [AllowAnonymous]
    public class EvaluationController : Controller
    {
        private IEvaluationService _evaluationService;
        private readonly ITrainerService _trainer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public EvaluationController(IEvaluationService evaluationService, IHttpContextAccessor httpContextAccessor, ITrainerService trainer, IConfiguration configuration)
        {
            _evaluationService = evaluationService;
            _httpContextAccessor = httpContextAccessor;
            _trainer = trainer;
            _configuration = configuration;
        }
        [Route("Evaluation/Index/{TrainerID}")]
        public IActionResult Index(string SearchByName, int TrainerID)
        {
            Trainer trainer21 = (Trainer)_trainer.Trainers.FirstOrDefault(s => s.ID == TrainerID);
            if (!trainer21.Loginstatus)
            {
                return RedirectToAction("Index", new RouteValueDictionary(
                   new { controller = "Home", action = "Index", Id = TrainerID }));
            }
            var result = _evaluationService.GetTrainerWithListOfEvaluationById(TrainerID);//0614e467-cb17-4f0a-9074-5c161ef39f84
            ViewData["SearchByName"] = SearchByName;
            if (SearchByName != null)
            {
                List<EvaluationTraineesDTO> test = (List<EvaluationTraineesDTO>)result.evaluationTraineesDTOs.Where(s => s.Name.ToLower().Contains(SearchByName.ToLower())).ToList();
                result.evaluationTraineesDTOs = test;
            }
            return View(result);
        }
        public IActionResult LogoutTr(int TrainerID)
        {
            _trainer.LogoutTrainer(TrainerID);
            return RedirectToAction("Index", new RouteValueDictionary(
            new { controller = "Home", action = "Index", Id = TrainerID }));
        }
        public IActionResult RatingPage(int traineeID, string TrainerID)
        {
            ViewData["TrainerID"] = TrainerID;
            Trainer trainer21 = (Trainer)_trainer.Trainers
                .Include(s => s.Trainees)
                .FirstOrDefault(s => s.ID.ToString() == TrainerID);
            if (!trainer21.Loginstatus)
            {
                return RedirectToAction("Index", new RouteValueDictionary(
                   new { controller = "Home", action = "Index", Id = TrainerID }));
            }
            foreach (var trainee in trainer21.Trainees)
            {
                if (traineeID == trainee.ID)
                {
                    var result = _evaluationService.getTraineeWithEvaluationForm(traineeID);
                    return View(result);
                }
            }
            return RedirectToAction("Index", new RouteValueDictionary(
            new { controller = "Home", action = "Index", Id = TrainerID }));

        }

        [HttpPost]
        public IActionResult RatingPage(evaluationRequestDto evaluation)
        {
            //if (ModelState.IsValid)
            //{
            //    _evaluationService.AddEvaluation(evaluation);
            //}

            //var result = _evaluationService.getTraineeWithEvaluationForm(evaluation.TraineeID);

            //return View(result);
            var evaluationRate = evaluation.Questions.Select(item => item.value).Sum();
            if (evaluationRate == 0)
            {
                ViewData["ErrorMassege"] = "Please Evaluate. ";
                ViewData["TrainerID"] = evaluation.TrainerID;
                var result = _evaluationService.getTraineeWithEvaluationForm(evaluation.TraineeID);
                return View(result);
            }

            _evaluationService.AddEvaluation(evaluation);
            return RedirectToAction("Index", new RouteValueDictionary(
         new { controller = "Evaluation", action = "Index", TrainerID = evaluation.TrainerID }));
        }
        public IActionResult EvaluationReportPDF(int id)
        {
            var result = _evaluationService.GetTrainerWithListOfEvaluationById(id);
            DataTable dt = new DataTable();

            dt.Columns.Add("Name");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("EvaluationRate");
            foreach (var trainees in result.evaluationTraineesDTOs)
            {
                if (trainees.EvaluationRate == 0)
                {
                    dt.Rows.Add(trainees.Name, trainees.SectionField, "No Evaluation");
                }
                else if (trainees.EvaluationRate <= 70)
                {
                    //Poor 
                    dt.Rows.Add(trainees.Name, trainees.SectionField, "Poor");
                }
                else if (trainees.EvaluationRate <= 79)
                {
                    // Good 
                    dt.Rows.Add(trainees.Name, trainees.SectionField, "Good");
                }
                else if (trainees.EvaluationRate <= 89)
                {
                    //Very Good 
                    dt.Rows.Add(trainees.Name, trainees.SectionField, "VeryGood ");
                }
                else
                {
                    // Excellent
                    dt.Rows.Add(trainees.Name, trainees.SectionField, "Excellent");
                }
            }


            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report6.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet2", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result1 = lr.Execute(RenderType.Pdf, 1, parameters, "");
            return new FileContentResult(result1.MainStream, "application/pdf");

        }
        public IActionResult EvaluationReportExcel(int id)
        {
            var result = _evaluationService.GetTrainerWithListOfEvaluationById(id);
            DataTable dt = new DataTable();

            dt.Columns.Add("Name");
            dt.Columns.Add("SectionField");
            dt.Columns.Add("EvaluationRate");
            foreach (var trainees in result.evaluationTraineesDTOs)
            {

                dt.Rows.Add(trainees.Name, trainees.SectionField, trainees.EvaluationRate);
            }
            var ReportPath = _configuration.GetValue<string>("ReportPath");
            var path = ReportPath + "\\Report6.rdlc";

            Dictionary<string, string> parameters = new Dictionary<string, string>();


            LocalReport lr = new LocalReport(path);
            lr.AddDataSource("DataSet2", dt);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            var result1 = lr.Execute(RenderType.Excel, 1, parameters, "");

            return new FileContentResult(result1.MainStream, "application/vnd.ms-excel");

        }
    }
}
