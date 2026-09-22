// Query параметрите за GET /api/ItsRolls.
// Описват филтри, сортиране и пагинация.

namespace OperativeService.DTOs;

public class ItsRollsQueryDto
{
    // Филтър: all | year | month | day
    public string Filter { get; set; } = "all";

    // Година (за filter=year, month, day)
    public int? Year { get; set; }

    // Месец (за filter=month, day)
    public int? Month { get; set; }

    // Ден (за filter=day)
    public int? Day { get; set; }

    // Първо сортиране: date | sum
    public string SortBy { get; set; } = "id";

    // Посока на първото сортиране: asc | desc
    public string Order { get; set; } = "asc";

    // Второ сортиране (по избор)
    public string? SortBy2 { get; set; }

    // Посока на второто сортиране
    public string? Order2 { get; set; }

    // Номер на страница (започва от 1)
    public int Page { get; set; } = 1;

    // Брой записи на страница
    public int PageSize { get; set; } = 10;
}