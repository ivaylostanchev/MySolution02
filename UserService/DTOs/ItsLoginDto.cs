// Данните, които клиентът праща при логин.

namespace UserService.DTOs;

public class ItsLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}