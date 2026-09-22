// Query параметрите за GET /api/ItsRolls.
// Валидират се автоматично от ASP.NET.

using System.ComponentModel.DataAnnotations;

namespace OperativeService.DTOs;

public class ItsRollsQueryDto
{
    [RegularExpression("^(all|year|month|day)$", ErrorMessage = "Filter must be: all, year, month, day")]
    public string Filter { get; set; } = "all";

    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
    public int? Year { get; set; }

    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
    public int? Month { get; set; }

    [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
    public int? Day { get; set; }

    [RegularExpression("^(id|date|sum)$", ErrorMessage = "SortBy must be: id, date, sum")]
    public string SortBy { get; set; } = "id";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "Order must be: asc, desc")]
    public string Order { get; set; } = "asc";

    [RegularExpression("^(id|date|sum)$", ErrorMessage = "SortBy2 must be: id, date, sum")]
    public string? SortBy2 { get; set; }

    [RegularExpression("^(asc|desc)$", ErrorMessage = "Order2 must be: asc, desc")]
    public string? Order2 { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
}