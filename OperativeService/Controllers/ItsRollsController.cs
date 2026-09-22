// Контролер за хвърляне на зарове.
// Всички endpoints са защитени с JWT токен.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OperativeService.Data;
using OperativeService.DTOs;
using OperativeService.Models;
using System.Security.Claims;

namespace OperativeService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItsRollsController : ControllerBase
{
    private readonly ItsAppDbContext _db;

    // DI: получаваме базата
    public ItsRollsController(ItsAppDbContext db)
    {
        _db = db;
    }

    // POST /api/ItsRolls
    // Хвърля два зара и записва резултата в базата
    [HttpPost]
    public async Task<IActionResult> Roll()
    {
        // Вземаме UserId от JWT токена
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? "0");

        // Хвърляме два зара (1-6)
        var die1 = Random.Shared.Next(1, 7);
        var die2 = Random.Shared.Next(1, 7);
        var sum = die1 + die2;

        // Създаваме запис
        var roll = new ItsDiceRoll
        {
            UserId = userId,
            Die1 = die1,
            Die2 = die2,
            Sum = sum,
            RolledAt = DateTime.UtcNow
        };

        _db.DiceRolls.Add(roll);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            die1 = die1,
            die2 = die2,
            sum = sum,
            rolledAt = roll.RolledAt
        });
    }

    // GET /api/ItsRolls
    // Връща хвърлянията на логнатия потребител с филтри, сортиране и пагинация
    [HttpGet]
    public async Task<IActionResult> GetRolls([FromQuery] ItsRollsQueryDto query)
    {
        // Вземаме UserId от JWT токена
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? "0");

        // Започваме с всички хвърляния на този потребител
        var rollsQuery = _db.DiceRolls.Where(r => r.UserId == userId);

        // ─── ФИЛТРИ ───
        if (query.Filter.Equals("year", StringComparison.OrdinalIgnoreCase) && query.Year.HasValue)
        {
            rollsQuery = rollsQuery.Where(r => r.RolledAt.Year == query.Year.Value);
        }
        else if (query.Filter.Equals("month", StringComparison.OrdinalIgnoreCase)
                 && query.Year.HasValue && query.Month.HasValue)
        {
            rollsQuery = rollsQuery.Where(r =>
                r.RolledAt.Year == query.Year.Value &&
                r.RolledAt.Month == query.Month.Value);
        }
        else if (query.Filter.Equals("day", StringComparison.OrdinalIgnoreCase)
                 && query.Year.HasValue && query.Month.HasValue && query.Day.HasValue)
        {
            rollsQuery = rollsQuery.Where(r =>
                r.RolledAt.Year == query.Year.Value &&
                r.RolledAt.Month == query.Month.Value &&
                r.RolledAt.Day == query.Day.Value);
        }
        // "all" → без филтър

        // ─── СОРТИРАНЕ ───
        // Проверяваме кои критерии участват (независимо на коя позиция са)
        bool sumIsInvolved =
            query.SortBy.Equals("sum", StringComparison.OrdinalIgnoreCase) ||
            (query.SortBy2?.Equals("sum", StringComparison.OrdinalIgnoreCase) ?? false);

        bool dateIsInvolved =
            query.SortBy.Equals("date", StringComparison.OrdinalIgnoreCase) ||
            (query.SortBy2?.Equals("date", StringComparison.OrdinalIgnoreCase) ?? false);

        if (sumIsInvolved && dateIsInvolved)
        {
            // Sum primary (секция ii), Date secondary (секция i)
            // Вземаме посоките от правилните полета
            var sumOrder = query.SortBy.Equals("sum", StringComparison.OrdinalIgnoreCase)
                ? query.Order
                : query.Order2 ?? "desc";

            var dateOrder = query.SortBy.Equals("date", StringComparison.OrdinalIgnoreCase)
                ? query.Order
                : query.Order2 ?? "asc";

            // Sum primary
            var ordered = sumOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? rollsQuery.OrderBy(r => r.Sum)
                : rollsQuery.OrderByDescending(r => r.Sum);

            // Date secondary
            rollsQuery = dateOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? ordered.ThenBy(r => r.RolledAt)
                : ordered.ThenByDescending(r => r.RolledAt);
        }
        else if (sumIsInvolved)
        {
            // Само Sum
            var sumOrder = query.SortBy.Equals("sum", StringComparison.OrdinalIgnoreCase)
                ? query.Order
                : query.Order2 ?? "desc";

            rollsQuery = sumOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? rollsQuery.OrderBy(r => r.Sum)
                : rollsQuery.OrderByDescending(r => r.Sum);
        }
        else if (dateIsInvolved)
        {
            // Само Date
            var dateOrder = query.SortBy.Equals("date", StringComparison.OrdinalIgnoreCase)
                ? query.Order
                : query.Order2 ?? "asc";

            rollsQuery = dateOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? rollsQuery.OrderBy(r => r.RolledAt)
                : rollsQuery.OrderByDescending(r => r.RolledAt);
        }
        else
        {
            // Default: Id ascending (стабилна пагинация)
            rollsQuery = rollsQuery.OrderBy(r => r.Id);
        }

        // ─── ПАГИНАЦИЯ ───
        var totalRecords = await rollsQuery.CountAsync();

        var rolls = await rollsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // ─── ОТГОВОР ───
        var response = new ItsPagedResponseDto<ItsDiceRoll>
        {
            Data = rolls,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling((double)totalRecords / query.PageSize)
        };

        return Ok(response);
    }
}