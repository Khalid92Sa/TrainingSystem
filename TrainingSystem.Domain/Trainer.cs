using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingSystem.Domain
{
    public class Trainer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public string Name { get; set; }
        public string SectionID { get; set; }
        public bool Status { get; set; }
        public int? ContactNumber { get; set; }
        public string Address { get; set; }
        public string UserName
        {
            get { return Name.Replace(" ", "_"); }
        }
        public string Email { get; set; }
        public string Password { get; set; }
        public Section Section { get; set; }
        public ICollection<Trainee> Trainees { get; set; }

    }
}
