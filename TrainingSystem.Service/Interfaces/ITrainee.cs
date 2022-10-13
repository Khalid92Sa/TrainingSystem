using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;

namespace TrainingSystem.Service
{
    public interface ITrainee
    {
        IQueryable<Trainee> Trainees { get; }
        public Trainee GetTraineeByID(int id);
        public Task CreateTrainee(Trainee trainee);
        public Task UpdateTraineeCV(string FileName, byte[] File, int id);
        public Task UpdateTrainee(Trainee trainee, int id);
        public void AddTraineeToSection(int id, int sectionid);
        public void RemoveTraineeFromSection(Section section);
        public Boolean RepetedName(string Name);
        public Boolean RepetedNameupdate(string Name, int id);
    }
}
