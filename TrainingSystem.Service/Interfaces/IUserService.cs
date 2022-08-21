using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Users;

namespace TrainingSystem.Service
{
    public interface IUserService
    {
        Task<Microsoft.AspNetCore.Identity.SignInResult> Passwordsignin(LoginDTO loginDTO);
        Task Logout();
        Task<ResponseDTO> ForgotPassword(ForgotPasswordDTo forgotPasswordDTo);
        int GenerateRandomNo();
        Boolean ConfirmationCode(String userName, int code);
        Task<IdentityUser> GetUserByUserName(string userName);
    }
}
