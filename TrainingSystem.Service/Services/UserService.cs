using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public UserService(ApplicationDbContext context, IUserRepository userRepository, IEmailSender emailSender, IMemoryCache memoryCache)
        {
            _UserRepository = userRepository;
            _context = context;
            _emailSender = emailSender;
            _iMemoryCache = memoryCache;
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
            var user =await _UserRepository.GetUserByUserName(forgotPasswordDTo.UserName);
            if (user != null)
            {
                //var code = GenerateRandomNo();
                var code = 1111;
                string subject = "confirmation Code";
                List<string> adresses = new List<string>() { user.Email };
                string body = "Please copy this code:" + code + "And Paste";
                var result = _emailSender.SendMail(subject,adresses,body);
                if (result.IsSuccess)
                {
                    string key = user.UserName;
                    var cacheEntryOptions = new MemoryCacheEntryOptions().
                        SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _iMemoryCache.Set(key,code, cacheEntryOptions);
                }
                return result;
            }
            else
            {
              
                return new ResponseDTO() { IsSuccess = false, message = "user not found " };
            }
            
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

        public Boolean ConfirmationCode(String userName,int code)
        {
            if (_iMemoryCache.Get<int>(userName) == code)
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
