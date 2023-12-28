using Microsoft.AspNetCore.Identity;

namespace Csv_Reader.Domain.Entity
{
    public class User : IdentityUser
    {
        public DateOnly Birthdate { get; set; }
    }
}