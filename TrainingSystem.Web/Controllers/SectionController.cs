using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TrainingSystem.Domain;
using TrainingSystem.Service;

namespace TrainingSystem.Web.Controllers
{
    public class SectionController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }
        
    }
}
