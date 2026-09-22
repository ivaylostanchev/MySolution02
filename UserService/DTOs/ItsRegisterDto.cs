// Данните, които клиентът праща при регистрация.
// Не съдържа Id или PasswordHash — те се създават от сървъра.

namespace UserService.DTOs;

public class ItsRegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}