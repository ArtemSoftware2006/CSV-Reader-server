using Csv_Reader.Domain.Entity;
using Csv_Reader.Domain.Exceptions;
using Csv_Reader.Domain.Security;
using Csv_Reader.Domain.ViewModel.User;
using Csv_Reader.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Csv_Reader.Services.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private bool LOCKOUT_ON_FAILURE = false;
        public AuthorizationService(UserManager<User> userManager, 
            SignInManager<User> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<bool> Login(LoginUserVm model)
        {
            SignInResult result = 
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, LOCKOUT_ON_FAILURE);
                
            if (result.Succeeded)
            {
                return true;
            }

            return false;
        }

        public async Task<User> Register(RegistrationUserVm model)
        {
            User user = new User() 
            {
                Email = model.Email,
                UserName = model.Email,
                PasswordHash = PasswordCoder.HashPassword(model.Password),
            };

            IdentityResult result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                return user;
            }

            throw new RegistrationException("Registration failed: " + string.Join(", ", result.Errors));
        }
    }
}