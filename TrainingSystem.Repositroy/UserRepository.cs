using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Users;
using TrainingSystem.Domain;

namespace TrainingSystem.Repositroy
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRepository(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> UserManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = UserManager;
            _roleManager = roleManager;
        }
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> Passwordsignin(LoginDTO loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                var userrfalid = await _signInManager.PasswordSignInAsync("sdksmcajscm", loginDto.Password, loginDto.RememberMe, false);
                return userrfalid;
            }
            var userr = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, loginDto.RememberMe, false);
            return userr;
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

      
        public async Task<IdentityUser> GetUserByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            //var user = await _userManager.FindByEmailAsync(userName);
            return user;
        }

    }
}
