using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Users;

namespace TrainingSystem.Repositroy
{
    public interface IUserRepository
    {
        Task<Microsoft.AspNetCore.Identity.SignInResult> Passwordsignin(LoginDTO loginDTO);
        Task Logout();
     
        Task<IdentityUser> GetUserByUserName(string userName);

    }
}
