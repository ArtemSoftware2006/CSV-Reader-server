
namespace Csv_Reader.Domain.Entity
{
    public class UserFile
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}