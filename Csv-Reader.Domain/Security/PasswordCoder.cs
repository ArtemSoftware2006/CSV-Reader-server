using System.Security.Cryptography;
using System.Text;

namespace Csv_Reader.Domain.Security
{
    public class PasswordCoder
    {
        public static string HashPassword(string password)
        {
            var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}