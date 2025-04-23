using WebApplication1.DTO;

namespace WebApplication1.Services.Iservices
{
    public interface IAccountService
    {

        Task<object> RegisterAsync(ApplicationUserDto userDto);
        Task<object> LoginAsync(LoginDto userVm);
    }
}
