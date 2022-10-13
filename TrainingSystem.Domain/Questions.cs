using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Domain
{
    public class Questions
    {
        [Required]
        public int id { get; set; }

        public string question { get; set; }

    }
}
