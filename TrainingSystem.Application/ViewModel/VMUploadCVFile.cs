using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.ViewModel
{
    public class VMUploadCVFile
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter file name")]
        public string FileName { get; set; }
        [Required(ErrorMessage = "Please select file Image Or Pdf")]
        public IFormFile File { get; set; }

    }
}
