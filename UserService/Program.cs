// Входна точка на приложението.
// Тук се конфигурират услугите (DI), middleware-ите и се стартира сървърът.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Data;

// Създаваме builder за приложението
var builder = WebApplication.CreateBuilder(args);

// Фиксираме HTTPS порта
builder.WebHost.UseUrls("https://localhost:7276");

// Регистрираме контролерите
builder.Services.AddControllers();

// Регистрираме Swagger генератора
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрираме базата данни с connection string от appsettings.json
builder.Services.AddDbContext<ItsAppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Вземаме JWT ключа от appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"]!;

// Регистрираме JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Казваме как да се валидира токенът
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Build-ваме приложението
var app = builder.Build();

// Регистрираме global exception middleware
app.UseMiddleware<UserService.Middleware.ItsExceptionMiddleware>();

// Създаваме scope и се уверяваме, че базата съществува
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ItsAppDbContext>();
    db.Database.EnsureCreated();
}

// Включваме Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

// Включваме authentication middleware (проверява токена)
app.UseAuthentication();

// Включваме authorization middleware (проверява права)
app.UseAuthorization();

// Насочваме заявките към контролерите
app.MapControllers();

// Стартираме сървъра
app.Run();