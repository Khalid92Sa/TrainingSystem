using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Users;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;
using TrainingSystem.Service.Interfaces;

namespace TrainingSystem.Service
{
   
    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private readonly IUserRepository _UserRepository;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _iMemoryCache;
        private readonly ITrainerService _trainerService;
        public UserService(ApplicationDbContext context, IUserRepository userRepository, IEmailSender emailSender, IMemoryCache memoryCache, ITrainerService trainerService)
        {
            _UserRepository = userRepository;
            _context = context;
            _emailSender = emailSender;
            _iMemoryCache = memoryCache;
            _trainerService = trainerService;
        }
        public Task<SignInResult> Passwordsignin(LoginDTO loginDTO)
        {
            return _UserRepository.Passwordsignin(loginDTO);
        }
        public async Task Logout()
        {
            await _UserRepository.Logout();
        }
        public async Task<ResponseDTO> ForgotPassword(ForgotPasswordDTo forgotPasswordDTo)
        {
            var user = await _UserRepository.GetUserByUserName(forgotPasswordDTo.UserName);
            if (user != null)
            {
                var code = GenerateRandomNo();
                Console.WriteLine(code);
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.To.Add(new MailAddress(user.Email));
                    message.From = new MailAddress("notifications@techprocess.net");
                    message.Subject = "Forget Password";
                    message.IsBodyHtml = true;
                    message.Body = String.Join(
                        Environment.NewLine,
                         "Dear " + user.UserName + ",\n",
                         "Confirmation code:" + code+"\n",
                         "Best Regards,");
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
                string key = user.UserName.ToLower();
                var cacheEntryOptions = new MemoryCacheEntryOptions().
                    SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _iMemoryCache.Set(key, code, cacheEntryOptions);
                return new ResponseDTO() { IsSuccess = true, message = "success" };
            }
            var trainer = _trainerService.Trainers.FirstOrDefault(s=>s.UserName==forgotPasswordDTo.UserName);
            if (trainer != null)
            {
                var code = GenerateRandomNo();
                Console.WriteLine(code);
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
                         "Confirmation code:" + code,
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
                string key = trainer.UserName.ToLower();
                var cacheEntryOptions = new MemoryCacheEntryOptions().
                    SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _iMemoryCache.Set(key, code, cacheEntryOptions);
                return new ResponseDTO() { IsSuccess = true, message = "TrainerSuccess" };
            }
            
             return new ResponseDTO() { IsSuccess = false, message = "User not found " };
            

        }


        public async Task<IdentityUser> GetUserByUserName(string userName)
        {
            var user = await _UserRepository.GetUserByUserName(userName);
            return user;
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public Boolean ConfirmationCode(String userName, int code)
        {
            userName=userName.ToLower();
            if (_iMemoryCache.Get<int>(userName) == code)
            //if (1111 == code)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
