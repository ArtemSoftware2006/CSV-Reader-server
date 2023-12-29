using Csv_Reader.DAL.Interfaces;
using Csv_Reader.Domain.Entity;
using Csv_Reader.Domain.ViewModel.File;
using Csv_Reader.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Csv_Reader.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly string PATH_TO_STORE;
        private readonly string EXTENSION = ".csv";
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

        public async Task<bool> DeleteFileAsync(string userId, Guid fileId)
        {
            try
            {
                string filePath = PATH_TO_STORE + fileId + EXTENSION;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    await _fileRepostory.DeleteAsync(fileId);
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

        public async Task<List<ShortUserFile>> GetFileNamesAsync(string userId)
        {
            try
            {
                User user = await _userManager.Users.Include(x => x.UserFiles).FirstOrDefaultAsync(x => x.Id == userId);
                
                if (user != null)
                {
                    return user.UserFiles
                        .Select(x => new ShortUserFile(x.Name, x.Id))
                        .ToList();
                }
                return new List<ShortUserFile>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                return new List<ShortUserFile>();
            }
        }

        public async Task<Stream> GetFileStreamAsync(string userId, Guid fileId)
        {
            try
            {
                string filePath = PATH_TO_STORE + fileId + EXTENSION;
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Файл не найден. (id = {fileId}, userId = {userId})", filePath);
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream;                    
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

        public async Task<bool> SaveFileAsync(string userId, IFormFile file)
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