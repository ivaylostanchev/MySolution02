// Връзката между C# кода и базата данни (EF Core).
// Тук се регистрират всички таблици (DbSet).

using Microsoft.EntityFrameworkCore;
using OperativeService.Models;

namespace OperativeService.Data;

public class ItsAppDbContext : DbContext
{
    // Конструкторът приема опции (connection string, доставчик)
    public ItsAppDbContext(DbContextOptions<ItsAppDbContext> options) : base(options) { }

    // Таблица DiceRolls в базата
    public DbSet<ItsDiceRoll> DiceRolls { get; set; }
}