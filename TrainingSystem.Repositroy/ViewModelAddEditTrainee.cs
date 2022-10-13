using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class ViewModelAddEditTrainee
    {
        public Trainee trainee { get; set; }
        public IFormFile File { get; set; }
    }
}
