// Модел на едно хвърляне на два зара.
// Описва структурата на един запис в таблицата DiceRolls.

namespace OperativeService.Models;

public class ItsDiceRoll
{
    // Уникален номер (първичен ключ)
    public int Id { get; set; }

    // ID на потребителя, който е хвърлил (от JWT токена)
    public int UserId { get; set; }

    // Резултат от първия зар (1-6)
    public int Die1 { get; set; }

    // Резултат от втория зар (1-6)
    public int Die2 { get; set; }

    // Сума на двата зара
    public int Sum { get; set; }

    // Кога е хвърлено
    public DateTime RolledAt { get; set; } = DateTime.UtcNow;
}