using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingSystem.Domain
{
    public class Section
    {
        [Range(0, 5)]
        public int ID { get; set; }
        public string idstring { 
            get {
                if (ID < 10)
                { return "SF-0" + ID; }
                
                return "SF-" + ID;
            } }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime EndDate {
            get{return StartDate.AddMonths(3);}
        }
        [ForeignKey("Trainer")]
        public string TrainerID { get; set; }
        public Trainer Trainer { get; set; }
        public ICollection<Trainee> Trainees { get; set; }
        public int SectionLookupID { get; set; }
        public SectionLookup SectionField { get; set; }
    }
}
