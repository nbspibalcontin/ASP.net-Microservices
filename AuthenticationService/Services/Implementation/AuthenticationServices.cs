using AuthenticationService.DTO;
using AuthenticationService.Exception;
using AuthenticationService.Services.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Services.Implementation
{
    public class AuthenticationServices : IAuthentication
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationServices(UserManager<IdentityUser> userManager, IMapper mapper,
            SignInManager<IdentityUser> signInManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
        }

        //Registration
        public async Task<MessageResponse> Register(RegisterDto register)
        {
            try
            {
                var user = _mapper.Map<IdentityUser>(register);

                IdentityUser userExist = await _userManager.FindByEmailAsync(register.Email);

                if (userExist != null)
                {
                    throw new ApplicationException("User with the provided email already exists.");
                }

                //Check if the role is already exist
                if (!await _roleManager.RoleExistsAsync(register.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(register.Role));                   
                }
                else
                {
                    throw new RoleAlreadyExist("User with the provided email already exists.");
                }

                //Create User
                var result = await _userManager.CreateAsync(user, user.PasswordHash!);
                //Assign User Role
                await _userManager.AddToRoleAsync(user, register.Role);

                if (!result.Succeeded) {
                    var errorMessages = string.Join(", ", result.Errors.Select(error => error.Description));
                    return new MessageResponse($"Failed to create user. Errors: {errorMessages}");
                }

                Claim[] userClaims =
                    [
                    new Claim(ClaimTypes.Email, register.Email),
                    new Claim(ClaimTypes.Role, register.Role!)
                    ];

                await _userManager.AddClaimsAsync(user, userClaims);

                return new MessageResponse("Registration Successfully");
            }
            catch (ApplicationException)
            {
                throw;
            }           
        }

        //Login
        public async Task<string> Login(LoginDto login)
        {
            try
            {
                if (string.IsNullOrEmpty(login.Email))
                {
                    throw new ArgumentNullException(nameof(login.Email), "Email cannot be null or empty.");
                }

                var userExist = await _userManager.FindByEmailAsync(login.Email);

                if (userExist == null)
                {
                    throw new ApplicationException("User with the provided email not found.");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(userExist, login.Password, false);

                if (!result.Succeeded)
                {
                    throw new ApplicationException("Failed to login");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                // Generate JWT token
                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: await _userManager.GetClaimsAsync(userExist),
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials
                );

                // Write and return the JWT token
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (ApplicationException)
            {
                throw;
            }
        }


    }
}
