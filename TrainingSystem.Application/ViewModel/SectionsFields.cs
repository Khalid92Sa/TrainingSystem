using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TrainingSystem.Application.ViewModel
{
    public class SectionsFields
    {
        public int ID { get; set; }
        public string SectionField { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        public string Year { get; set; }
    }
}
