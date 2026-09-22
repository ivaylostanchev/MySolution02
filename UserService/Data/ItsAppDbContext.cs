// Връзката между C# кода и базата данни (EF Core).
// Тук се регистрират всички таблици (DbSet) и правилата за тях.

using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class ItsAppDbContext : DbContext
{
    // Конструкторът приема опции (connection string, доставчик)
    public ItsAppDbContext(DbContextOptions<ItsAppDbContext> options) : base(options) { }

    // Таблица Users в базата
    public DbSet<ItsUser> Users { get; set; }

    // Конфигурация на модела
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Email трябва да е уникален — двама потребители не могат да имат един и същ имейл
        modelBuilder.Entity<ItsUser>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}