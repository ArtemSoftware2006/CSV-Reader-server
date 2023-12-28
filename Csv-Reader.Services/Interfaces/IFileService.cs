using System.Runtime.InteropServices;

namespace Csv_Reader.Services.Interfaces
{
    public interface IFileService
    {
        Task<List<string>> GetFileNames(string userId);
        Task<bool> SaveFile(string userId, IFormFile file);
        Task<bool> DeleteFile(string userId, Guid fileId);
        Task<Stream> GetFileStream(string userId, Guid fileId);
    }
}