using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompanyApi.DTOs;
using CompanyApi.Models;

[ApiController]
[Route("[controller]")]
public class AuntificationController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuntificationController(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto userDto)
    {
        var user = new User
        {
            UserName = userDto.UserName,
            Email = userDto.Email,
            Login = userDto.UserName, // Assuming 'Login' is a required unique identifier
            Name = userDto.Name ?? "Default Name",
            PhoneNumber = userDto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (result.Succeeded)
        {
            return Ok();
        }

        var errors = result.Errors.Select(e => e.Description);
        return BadRequest(new { Errors = errors });
    }
    

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        // Находим пользователя по имени пользователя
        var user = await _userManager.FindByNameAsync(userLoginDto.UserName);

        // Проверяем пароль и создаем токен, если пароль верен
        if (user != null && await _userManager.CheckPasswordAsync(user, userLoginDto.Password))
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var secretKey = _configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                return Unauthorized("JWT secret key is not set in the configuration");
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        // Возвращаем статус 401 Unauthorized, если аутентификация не удалась
        return Unauthorized();
    }
}
