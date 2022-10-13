using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Trainers;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;

namespace TrainingSystem.Service
{
    public class TrainerService : ITrainerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITrainerRepository _TrainerRepository;

        public TrainerService(ITrainerRepository trainerRepository , ApplicationDbContext context)
        {
            _TrainerRepository = trainerRepository;
            _context = context;
        }

        public IQueryable<Trainer> Trainers => _TrainerRepository.Trainers;

        public  void AddTrainer(Trainer trainer)
        {
             _TrainerRepository.AddTrainer(trainer);
            
        }

        public async Task SaveChangesAsyncc()
        {
            await _TrainerRepository.SaveChangesAsyncc();
        }
        public async Task<Trainer> GetTrainerById(int id)=>await _TrainerRepository.GetTrainerById(id);
        public void UpdateTrainer(int id, Trainer trainer)
        {
              _TrainerRepository.UpdateTrainer(id, trainer);

             
        }

        public bool Login(string email, string password, int TrainerID)
        {
            var trainer =  _context.Trainers.FirstOrDefault(s=>s.UserName== email && s.Password== password && s.ID== TrainerID);
            if(trainer == null)
            { return false; }
            else {
                trainer.Loginstatus = true;
                _context.SaveChanges();
                return true;
            }
        }

        public void LogoutTrainer(int TrainerID)
        {
            var trainer = _context.Trainers.FirstOrDefault(s =>s.ID == TrainerID);
            trainer.Loginstatus = false;
            _context.SaveChanges();
        }

        public void SendLoginInfo(Trainer trainer)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.To.Add(new MailAddress(trainer.Email));
                message.From = new MailAddress("notifications@techprocess.net");
                message.Subject = "Mr.\\Mrs. " + trainer.Name + ",";
                message.IsBodyHtml = true;
                message.Body = String.Join(
                    Environment.NewLine,
                    "You Added to the training system as a trainer",
                    "UserName:" + trainer.UserName,
                    "Password:" + trainer.Password,
                    "Regards,");
                smtp.Host = "mail.sssprocess.com";
                smtp.Credentials = new NetworkCredential("notifications", "P@ssw0rd", "sss-process.org");
                smtp.Port = 587;
                smtp.EnableSsl = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {
            }
        }
        public Boolean RepetedName(string Name)
        {
            var trainers = _context.Trainers.FirstOrDefault(x => x.UserName == Name);
            if (trainers == null)
            {
                return true;
            }
            return false;
        }
        public Boolean RepetedNameupdate(string Name, int id)
        {
            var trainerToUpdate = _context.Trainers.FirstOrDefault(x => x.ID == id);
            if (trainerToUpdate.UserName == Name)
            {
                return true;
            }
            var trainees = _context.Trainers.FirstOrDefault(x => x.UserName == Name);
            if (trainees == null)
            {
                return true;
            }
            return false;
        }
    }
}
