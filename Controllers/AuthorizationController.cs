using Csv_Reader.Domain.Entity;
using Csv_Reader.Domain.Exceptions;
using Csv_Reader.Domain.ViewModel.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSV_Reader_server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly Csv_Reader.Services.Interfaces.IAuthorizationService _authService;
        private  readonly ILogger<AuthorizationController> _logger;
        public AuthorizationController(Csv_Reader.Services.Interfaces.IAuthorizationService authService,
            ILogger<AuthorizationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserVm model)
        {
            try
            {
                string token = await _authService.LoginAsync(model);   
                return Ok(new { token = token });
            }
            catch (LoginException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] RegistrationUserVm model)
        {
            try
            {
                User result = await _authService.RegisterAsync(model); 
                
                LoginUserVm loginUserVm = new LoginUserVm()
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = false,
                };

                string token = await _authService.LoginAsync(loginUserVm);

                return Ok(new { token = token });
            }
            catch (RegistrationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (LoginException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}