using Csv_Reader.DAL.Interfaces;
using Csv_Reader.Domain.Entity;
using Csv_Reader.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Csv_Reader.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly string PATH_TO_STORE;
        private readonly IUserFileRepository _fileRepostory;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<FileService> _logger;
        public FileService(IUserFileRepository fileRepostory,
            IConfiguration configuration,
            ILogger<FileService> logger,
            UserManager<User> userManager)
        {
            PATH_TO_STORE = configuration.GetSection("PathToStore").Value 
                ?? Directory.GetCurrentDirectory() + "/Store/";

            if (!Directory.Exists(PATH_TO_STORE))
            {
                Directory.CreateDirectory(PATH_TO_STORE);
            }
            
            _userManager = userManager;
            _logger = logger;
            _fileRepostory = fileRepostory;
        }

        public async Task<bool> DeleteFile(string userId, Guid fileId)
        {
            try
            {
                string filePath = PATH_TO_STORE + fileId;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                throw new FileNotFoundException($"Файл не найден. (id = {fileId}, userId = {userId})", filePath); 
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return false;
            }
        }

        public async Task<List<string>> GetFileNames(string userId)
        {
            try
            {
                User user = await _userManager.Users.Include(x => x.UserFiles).FirstOrDefaultAsync(x => x.Id == userId);
                
                if (user != null)
                {
                    return user.UserFiles
                        .Select(x => x.Name)
                        .ToList();
                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return new List<string>();
            }
        }

        public async Task<Stream> GetFileStream(string userId, Guid fileId)
        {
            try
            {
                string filePath = PATH_TO_STORE + fileId;
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Файл не найден. (id = {fileId}, userId = {userId})", filePath);
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return fileStream;                    
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Stream.Null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return Stream.Null;
            }
        }

        public async Task<bool> SaveFile(string userId, IFormFile file)
        {
            try
            {
                UserFile userFile = new UserFile()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Created = DateTime.UtcNow,
                    Name = file.FileName
                };

                await _fileRepostory.CreateAsync(userFile);

                using (var stream = new FileStream(PATH_TO_STORE + userFile.Id + Path.GetExtension(file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return true;   
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return false;
            }
        }
    }
}