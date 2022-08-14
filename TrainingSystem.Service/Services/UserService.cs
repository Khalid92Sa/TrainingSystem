using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingSystem.Application.DTOs.Users;
using TrainingSystem.Domain;
using TrainingSystem.Repositroy;

namespace TrainingSystem.Service
{
    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private readonly IUserRepository _UserRepository;

        public UserService(ApplicationDbContext context, IUserRepository userRepository)
        {
            _UserRepository = userRepository;
            _context = context;
        }
        public Task<SignInResult> Passwordsignin(LoginDTO loginDTO)
        {
            return _UserRepository.Passwordsignin(loginDTO);
        }
        public async Task Logout()
        {
            await _UserRepository.Logout();
        }
    }
}
