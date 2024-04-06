using AuthenticationService.DTO;
using AuthenticationService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthentication _authentication;

        public AuthenticationController(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            try
            {
                var response = await _authentication.Register(registerDto);

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error when registering the user: " + ex);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            try
            {
                var response = await _authentication.Login(loginDto);

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error when logining the user: " + ex);
            }
        }
    }
}
