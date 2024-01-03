using System.IdentityModel.Tokens.Jwt;
using Csv_Reader.Domain.Validators;
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
        public async Task<IActionResult> Create([FromForm][ExtensionValidator([".csv"])] IFormFile file) 
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
                string userId = GetUserId(token);

                bool result = await _fileService.SaveFileAsync(userId, file);

                if (!result)
                {
                    return BadRequest("Файл не сохранен");
                }

                return Ok();   
            }

            return BadRequest(ModelState);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            string userId = GetUserId(token);

            bool result = await _fileService.DeleteFileAsync(userId, id);

            if (!result)
            {
                return BadRequest("Файл не найден");
            }

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            string userId = GetUserId(token);

            return Ok(await _fileService.GetFileNamesAsync(userId));
        }
        [HttpGet]
        public async Task<IActionResult> GetFile(Guid id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            
            string userId = GetUserId(token);
            
            Stream file = await _fileService.GetFileStreamAsync(userId, id);

            if (file.Length == 0)
            {
                return BadRequest("Файл не найден");
            }

            return File(file, "application/octet-stream");
        }

        private string GetUserId(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token);

            string userId = jwtToken.Claims.First(c => c.Type == "userId").Value ?? "";

            return userId;
        }
    }
}