using Csv_Reader.Domain.ViewModel.File;

namespace Csv_Reader.Services.Interfaces
{
    public interface IFileService
    {
        Task<List<ShortUserFile>> GetFileNamesAsync(string userId);
        Task<bool> SaveFileAsync(string userId, IFormFile file);
        Task<bool> DeleteFileAsync(string userId, Guid fileId);
        Task<Stream> GetFileStreamAsync(string userId, Guid fileId);
    }
}