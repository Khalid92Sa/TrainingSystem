using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Domain;
using TrainingSystem.Service;

namespace TrainingSystem.Service
{
    public class TraineeService : ITrainee
    {
        private readonly ApplicationDbContext context;
        public TraineeService(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Trainee> Trainees => context.Trainees;

        public Trainee GetTraineeByID(int id)
        {
            return context.Trainees.FirstOrDefault(x => x.ID == id);
        }

        public async Task CreateTrainee(Trainee trainee)
        {
            context.Trainees.Add(trainee);
            await context.SaveChangesAsync();
        }     

        public async Task UpdateTrainee(Trainee trainee, int id)
        {
            var traineeToUpdate = GetTraineeByID(id);
            if (traineeToUpdate == null)
            {
                return;
            }
            traineeToUpdate.Name = trainee.Name;
            traineeToUpdate.Email = trainee.Email;
            traineeToUpdate.ContactNumber = trainee.ContactNumber;
            traineeToUpdate.StartDate = trainee.StartDate;
            traineeToUpdate.GraduationDate = trainee.GraduationDate;
            traineeToUpdate.SectionLookupID = trainee.SectionLookupID;
            traineeToUpdate.TrainerID = trainee.TrainerID;
            traineeToUpdate.HRFeedback = trainee.HRFeedback;
            await context.SaveChangesAsync();
        }

        public async Task UpdateTraineeCV(string FileName, byte[] File, int id)
        {
            var traineeToUpdate = GetTraineeByID(id);
            if (traineeToUpdate == null)
            {
                return;
            }
            traineeToUpdate.CV = File;
            traineeToUpdate.CVFileName = FileName;
            await context.SaveChangesAsync();
        }
        public void AddTraineeToSection(int id, int sectionid)
        {
            var traineeToUpdate = GetTraineeByID(id);
            traineeToUpdate.SectionID = sectionid;
            //await context.SaveChangesAsync();
            context.SaveChanges();
        }

        public void RemoveTraineeFromSection(Section section)
        {
            foreach (var trainee in Trainees)
            {
                if(trainee.SectionID == section.ID)
                {
                    trainee.SectionID = null;
                }  
            }
            //await context.SaveChangesAsync();
            context.SaveChanges();

        }

        public Boolean RepetedName(string Name)
        {
            var trainees= context.Trainees.FirstOrDefault(x => x.Name==Name);
            if(trainees == null)
            {
                return true;
            }
            return false;
        }
        public Boolean RepetedNameupdate(string Name, int id)
        {
            var traineeToUpdate = GetTraineeByID(id);
            if (traineeToUpdate.Name == Name)
            {
                return true; 
            }
            var trainees = context.Trainees.FirstOrDefault(x => x.Name == Name);
            if (trainees == null)
            {
                return true;
            }
            return false;
        }
    }
}
