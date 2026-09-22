// Данните, които клиентът праща при регистрация.
// Атрибутите карат ASP.NET да валидира автоматично.

using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class ItsRegisterDto
{
    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50, ErrorMessage = "First name must be at most 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50, ErrorMessage = "Last name must be at most 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100, ErrorMessage = "Email must be at most 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(100, ErrorMessage = "Password must be at most 100 characters")]
    public string Password { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}