using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services.Iservices;
using WebApplication1.Utility;

namespace WebApplication1.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            ) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<object> RegisterAsync(ApplicationUserDto userDto)
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.adminRole));
                await _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole));
            }

            var user = new ApplicationUser()
            {
                FullName = userDto.FullName,
                Email = userDto.Email,
                UserName = userDto.Email            
            };
            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                if (_userManager.Users.Count() == 1)
                {
                    await _userManager.AddToRoleAsync(user, SD.adminRole);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.CustomerRole);
                }

                return new { Message = "Registration successful" };
            }

            return new { Errors = result.Errors.Select(e => e.Description) };
        }



        public async Task<object> LoginAsync(LoginDto userVm)
        {
            var user = await _userManager.FindByEmailAsync(userVm.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, userVm.password))
            {
                return new { Message = "Invalid credentials" };
            }

            var claims = new List<Claim> {

              new Claim(ClaimTypes.Name, user.FullName),
              new Claim(ClaimTypes.Email, user.Email),
              new Claim(ClaimTypes.NameIdentifier, user.Id),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
              };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddDays(14),
                signingCredentials: creds
            );

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
        }

    }
}

