using System;
using System.Collections.Generic;
using System.Text;
using TrainingSystem.Application.DTOs.Users;

namespace TrainingSystem.Service.Interfaces
{
    public interface IEmailSender
    {
        ResponseDTO SendMail(string subject, List<string> adresses, string body);
    }
}
