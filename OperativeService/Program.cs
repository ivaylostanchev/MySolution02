// Входна точка на приложението.
// Конфигурира DI, middleware-и и стартира сървъра.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OperativeService.Data;

// Създаваме builder за приложението
var builder = WebApplication.CreateBuilder(args);

// Фиксираме HTTPS порта
builder.WebHost.UseUrls("https://localhost:7291");

// Регистрираме контролерите
builder.Services.AddControllers();

// CORS — позволява заявки от други портове (HTML-ът вика API-то)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Регистрираме Swagger генератора
builder.Services.AddEndpointsApiExplorer();

// Регистрираме Swagger с JWT поддръжка
builder.Services.AddSwaggerGen(options =>
{
    // Дефинираме как изглежда JWT authentication
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Въведи JWT токен (без 'Bearer ' префикс)"
    });

    // Казваме на Swagger да изисква токен за защитените endpoints
    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// Регистрираме базата данни с connection string от appsettings.json
builder.Services.AddDbContext<ItsAppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Вземаме JWT ключа от appsettings.json (същия като в UserService)
var jwtKey = builder.Configuration["Jwt:Key"]!;

// Регистрираме JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

// Създаваме базата, ако не съществува
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ItsAppDbContext>();
    db.Database.EnsureCreated();
}

// Global exception middleware (най-отпред)
app.UseMiddleware<OperativeService.Middleware.ItsExceptionMiddleware>();

// CORS (преди static files)
app.UseCors();

// Статични файлове от wwwroot
app.UseStaticFiles();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Authentication + Authorization
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.MapControllers();

// Стартираме сървъра
app.Run();