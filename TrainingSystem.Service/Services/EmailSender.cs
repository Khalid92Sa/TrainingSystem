using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using TrainingSystem.Application.DTOs.Users;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Service.Services
{
    public class EmailSender : IEmailSender
    {
        public ResponseDTO SendMail(string subject,List<string> adresses, string body)
        {
            MailMessage mail = new MailMessage();
            mail.Subject = subject;
            mail.From = new MailAddress("testmb651@gmail.com");
            foreach(var item in adresses)
            {
                mail.To.Add(item);
            }
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                var netCre = new NetworkCredential("testmb651@gmail.com", "Testtest1234");
                smtp.Credentials = netCre;
                smtp.Send(mail);
                smtp.Dispose();
                return new ResponseDTO() {IsSuccess=true,message="success" };
            }
            catch (Exception ex)
            {
                 return new ResponseDTO() { IsSuccess = false, message =ex.Message  }; 
            }
        }
    }
}
