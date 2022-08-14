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
            var userr = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, false);
            return userr;
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
