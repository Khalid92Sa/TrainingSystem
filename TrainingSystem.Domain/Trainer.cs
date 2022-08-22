using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingSystem.Domain
{
    public class Trainer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public string Name { get; set; }

        public bool Status { get; set; }
        public int? ContactNumber { get; set; }
        public string Address { get; set; }
        public string UserName
        {
            get { return Name.Replace(" ", "_"); }
        }
        public string Email { get; set; }
        [ForeignKey("Section")]
        public int? SectionID { get; set; }
        public Section Section { get; set; }
        public ICollection<Trainee> Trainees { get; set; }
        public string Password { get; set; }


    }
}