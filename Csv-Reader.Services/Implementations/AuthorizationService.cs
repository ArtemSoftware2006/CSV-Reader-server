using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Csv_Reader.Domain.Entity;
using Csv_Reader.Domain.Exceptions;
using Csv_Reader.Domain.ViewModel.User;
using Csv_Reader.Services.Interfaces;
using CSV_Reader_server.Services.Configs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<string> LoginAsync(LoginUserVm model)
        {
            SignInResult result = 
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, LOCKOUT_ON_FAILURE);
                
            if (result.Succeeded)
            {
                ClaimsIdentity identity = await GetIdentityAsync(model.Email, model.Password);

                var now = DateTime.UtcNow;

                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return encodedJwt;
            }

            throw new LoginException($"Пользователь с email {model.Email} не найден или неверный пароль");
        }

        public async Task<User> RegisterAsync(RegistrationUserVm model)
        {
            User user = new User() 
            {
                Email = model.Email,
                UserName = model.Email,
                //PasswordHash = PasswordCoder.HashPassword(model.Password),
                PasswordHash = model.Password,
                LockoutEnabled = false
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return user;
            }

            throw new RegistrationException("Registration failed: " + string.Join(", ", result.Errors));
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(string email, string password)
        {
            User user = await _userManager.FindByEmailAsync(email);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                List<Claim> claims = new List<Claim>() 
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    //TODO Как-то надо получать роли
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
                    new Claim("userId", user.Id.ToString())
                };

                ClaimsIdentity claimsIdentity = 
                    new ClaimsIdentity(claims, "token", 
                                            ClaimsIdentity.DefaultNameClaimType, 
                                            ClaimsIdentity.DefaultRoleClaimType
                                        );
                return claimsIdentity;
            } 

            throw new LoginException($"Пользователь с email {email} не найден или неверный пароль");
        }
    }
}