using WebApplication1.DTO;

namespace WebApplication1.HttpServices.Interface
{
    public interface IAuthentication
    {
        //Login
        Task<string> AuthenticationLogin(LoginDto loginDto);
    }
}
