using AuthenticationService.DTO;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace AuthenticationService.Services.Interface
{
    public interface IAuthentication
    {
        //Register
        Task<MessageResponse> Register(RegisterDto register);
        Task<string> Login(LoginDto login);
    }
}
