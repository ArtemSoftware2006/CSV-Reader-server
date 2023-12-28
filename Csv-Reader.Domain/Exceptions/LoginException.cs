namespace Csv_Reader.Domain.Exceptions
{
    public class LoginException : Exception
    {
        public LoginException(string message)
            :base(message) {}
    }
}