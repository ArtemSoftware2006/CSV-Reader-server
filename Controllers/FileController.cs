using System.IdentityModel.Tokens.Jwt;
using Csv_Reader.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSV_Reader_server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] IFormFile file) 
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            string userId = jwtToken.Claims.First(c => c.Type == "userId").Value;

            bool result = await _fileService.SaveFile(userId, file);

            if (!result)
            {
                return BadRequest("Файл не сохранен");
            }

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            string userId = jwtToken.Claims.First(c => c.Type == "userId").Value;

            return Ok(await _fileService.GetFileNames(userId));
        }
    }
}