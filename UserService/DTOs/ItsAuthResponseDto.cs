// Това, което сървърът връща след успешен логин.

namespace UserService.DTOs;

public class ItsAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
}