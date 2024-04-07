using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.HttpServices.Interface;

namespace WebApplication1.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthentication _authentication;

        public AuthenticationController(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            try
            {
                var result = await _authentication.AuthenticationLogin(loginDto);

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error" + ex);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when accessing the login service: " + ex);
            }
        }
    }
}
