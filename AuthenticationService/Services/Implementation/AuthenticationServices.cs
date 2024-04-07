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

                var userExist = await _userManager.FindByEmailAsync(register.Email);

                if (userExist != null)
                {
                    throw new ApplicationException("User with the provided email already exists.");
                }

                // Create User
                var result = await _userManager.CreateAsync(user, user.PasswordHash!);

                if (!result.Succeeded)
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(error => error.Description));
                    return new MessageResponse($"Failed to create user. Errors: {errorMessages}");
                }

                // Check if the role is already exist, if not create it
                if (!await _roleManager.RoleExistsAsync("admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    // Assign "admin" Role to the First User
                    await _userManager.AddToRoleAsync(user, "admin");
                }
                else
                {
                    // If "admin" role exists, assign "user" role to the user
                    await _roleManager.CreateAsync(new IdentityRole("user"));
                    await _userManager.AddToRoleAsync(user, "user");
                }

                var userRoles = await GetUserRole(register.Email);

                // Add Claims
                var userClaims = new Claim[]
                {
                  new Claim(ClaimTypes.Email, register.Email),
                  new Claim(ClaimTypes.Role, userRoles)
                };

                await _userManager.AddClaimsAsync(user, userClaims);

                return new MessageResponse("Registration Successful");
            }
            catch (System.Exception ex)
            {
                // Log exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Rethrow the exception
            }
        }

        //Get User role
        public async Task<string> GetUserRole(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                throw new ApplicationException($"User with email {userEmail} not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                // If the user has roles, return the first one
                return roles.First();
            }
            else
            {
                // If the user has no roles, return an empty string or handle it as needed
                return string.Empty;
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

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
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
