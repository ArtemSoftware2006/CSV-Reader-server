using Csv_Reader.Domain.Entity;
using Csv_Reader.Domain.ViewModel.User;

namespace Csv_Reader.Services.Interfaces
{
    public interface IAuthorizationService
    {
        Task<User> Register(RegistrationUserVm model);
        Task<bool> Login(LoginUserVm model);
    }
}