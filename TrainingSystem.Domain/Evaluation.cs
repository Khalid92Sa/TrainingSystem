using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Domain
{
    public class Evaluation
    {
        public int ID { get; set; }
        public int EvaluationRate { get; set; }

        public string feedback { get; set; }
        public Trainer Trainer { get; set; }
        public int TraineeID { get; set; }
        public Trainee Trainee { get; set; }
    }
}
