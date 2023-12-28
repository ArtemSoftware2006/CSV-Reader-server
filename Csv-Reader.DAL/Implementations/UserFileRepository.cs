using Csv_Reader.DAL.Interfaces;
using Csv_Reader.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Csv_Reader.DAL.Implementations
{
    public class UserFileRepository : IUserFileRepository
    {
        private readonly AppDbContext _context;
        public UserFileRepository(AppDbContext context)
        {
            _context = context;
        } 
        public async Task<UserFile> CreateAsync(UserFile userFile)
        {
            await _context.UserFiles.AddAsync(userFile);

            await _context.SaveChangesAsync();

            return userFile;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            UserFile file = await _context.UserFiles.FirstOrDefaultAsync(x => x.Id == id);

            if (file is null)
            {
                return false;
            }

            _context.UserFiles.Remove(file);

            int result = await _context.SaveChangesAsync();
            
            return result > 0;
        }

        public async Task<UserFile> GetAsync(Guid id)
        {
            return await _context.UserFiles.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}