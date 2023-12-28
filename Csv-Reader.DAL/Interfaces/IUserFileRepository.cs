using Csv_Reader.Domain.Entity;

namespace Csv_Reader.DAL.Interfaces
{
    public interface IUserFileRepository
    {
        Task<UserFile> CreateAsync(UserFile userFile);
        Task<UserFile> GetAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}