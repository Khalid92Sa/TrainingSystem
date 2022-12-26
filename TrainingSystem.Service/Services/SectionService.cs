using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TrainingSystem.Application.ViewModel;
using TrainingSystem.Domain;

namespace TrainingSystem.Service
{
    public class SectionService : ISection
    {
        private readonly ApplicationDbContext context;
        public SectionService(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Section> Sections =>
            context.Sections;

        public Section GetSectionByID(int id)
        {
            return context.Sections.FirstOrDefault(x => x.ID == id);
        }

        public async Task CreateSection(Section section)
        {
            context.Sections.Add(section);
            await context.SaveChangesAsync();
            var sectionupdateTrainer = context.Sections.Max(i => i.ID);
            Trainer trainer = context.Trainers.First(s => s.ID == section.TrainerID);
            trainer.SectionID = sectionupdateTrainer;
            await context.SaveChangesAsync();
        }



        public async Task UpdateSection(Section section, int id)
        {
            var sectionToUpdate = GetSectionByID(id);
            if (sectionToUpdate == null)
            {
                return;
            }
            sectionToUpdate.SectionLookupID = section.SectionLookupID;
            sectionToUpdate.StartDate = section.StartDate;


            if (section.TrainerID != sectionToUpdate.TrainerID)
            {
                Trainer AddSectionIdTrainer = context.Trainers.First(s => s.ID == section.TrainerID);
                Trainer RemoveSectionIdTrainer = context.Trainers.First(s => s.ID == sectionToUpdate.TrainerID);
                AddSectionIdTrainer.SectionID = sectionToUpdate.ID;
                RemoveSectionIdTrainer.SectionID = null;
            }
            sectionToUpdate.TrainerID = section.TrainerID;
            await context.SaveChangesAsync();
        }

        public void SendEvaluationEmail(Section Section)
        {
            string trainees = @"";
            int count = 1;
            foreach (var x in Section.Trainees)
            {
                trainees += count + ")" + x.Name + "\n";
                count++;
            }
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.To.Add(new MailAddress(Section.Trainer.Email));
                message.From = new MailAddress("notifications@techprocess.net");
                message.Subject = "Evaluate Trainees";
                message.IsBodyHtml = false;
                message.Body = 
                    "Mr.\\Mrs. " + Section.Trainer.Name + ",\n\n"+
                     "Please Evaluate following trainees:\n"+
                     trainees+ "\n"+
                     "By The Link: " + "https://saib-web/TS/Home/Index/" + Section.Trainer.ID+"\n\n"+
                     "Best Regards,\n";
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

        public void SendStartEmail(Section Section)
        {
            string trainees = @"";
            int count = 1;
            foreach (var x in Section.Trainees)
            {
                trainees += count + ")" + x.Name + " Email:" + x.Email + "\n";
                count++;
            }
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.To.Add(new MailAddress(Section.Trainer.Email));
                message.From = new MailAddress("notifications@techprocess.net");
                message.Subject = "New Trainaing Section";
                message.IsBodyHtml = false;
                message.Body = 
                    "Mr.\\Mrs. " + Section.Trainer.Name + ",\n\n"+
                    "Our company has opened a new training track.\n"+
                    "You have been handed the " + Section.SectionField.SectionField + " Track with each of the following trainees:\n"+
                     trainees+"\n"+
                    "Best Regards,\n";
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


    }
}
