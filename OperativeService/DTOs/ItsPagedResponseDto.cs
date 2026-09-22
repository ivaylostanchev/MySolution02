// Обвивка за пагиниран отговор.
// Съдържа данните + информация за страницата.

namespace OperativeService.DTOs;

public class ItsPagedResponseDto<T>
{
    // Данните на текущата страница
    public List<T> Data { get; set; } = new();

    // Номер на текущата страница
    public int Page { get; set; }

    // Брой записи на страница
    public int PageSize { get; set; }

    // Общ брой записи (преди пагинация)
    public int TotalRecords { get; set; }

    // Общ брой страници
    public int TotalPages { get; set; }
}