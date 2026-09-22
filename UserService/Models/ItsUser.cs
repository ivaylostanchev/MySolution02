// Модел на потребител. Описва структурата на един запис в таблицата Users.
// Всяко свойство = една колона в базата.

namespace UserService.Models;

public class ItsUser
{
    // Уникален номер (първичен ключ)
    public int Id { get; set; }

    // Име на потребителя
    public string FirstName { get; set; } = string.Empty;

    // Фамилия на потребителя
    public string LastName { get; set; } = string.Empty;

    // Имейл (уникален в базата)
    public string Email { get; set; } = string.Empty;

    // Хеширана парола (BCrypt), НЕ самата парола
    public string PasswordHash { get; set; } = string.Empty;

    // Път до снимката (може да е null, ако няма качена)
    public string? ImagePath { get; set; }
}