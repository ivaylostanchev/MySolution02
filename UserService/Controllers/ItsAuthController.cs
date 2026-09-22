// Контролер за регистрация и логин.
// Създава потребители и връща JWT токени.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Data;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItsAuthController : ControllerBase
{
    private readonly ItsAppDbContext _db;
    private readonly IConfiguration _config;

    // DI: получаваме базата и конфигурацията
    public ItsAuthController(ItsAppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST /api/ItsAuth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] ItsRegisterDto dto)
    {
        // Проверяваме дали имейлът вече съществува
        var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
            return BadRequest(new { message = "Email already exists" });

        // Хешираме паролата с BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Ако има качена снимка — запазваме я
        string? imagePath = null;
        if (dto.Image != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
            var folder = Path.Combine("wwwroot", "images");
            Directory.CreateDirectory(folder);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            imagePath = "/images/" + fileName;
        }

        // Създаваме потребителя
        var user = new ItsUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = passwordHash,
            ImagePath = imagePath
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "User created successfully" });
    }

    // POST /api/ItsAuth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ItsLoginDto dto)
    {
        // Търсим потребителя по имейл
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        // Проверяваме паролата с BCrypt
        var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isValid)
            return Unauthorized(new { message = "Invalid email or password" });

        // Създаваме JWT токен
        var token = GenerateJwtToken(user);

        return Ok(new ItsAuthResponseDto { Token = token });
    }
    //// GET /api/ItsAuth/test-error
    //[HttpGet("test-error")]
    //public IActionResult TestError()
    //{
    //    throw new Exception("This is a test error");
    //}

    // Помощен метод за създаване на JWT
    private string GenerateJwtToken(ItsUser user)
    {
        // Вземаме тайния ключ от appsettings.json
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        // Данните, които слагаме в токена
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Създаваме токена
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}