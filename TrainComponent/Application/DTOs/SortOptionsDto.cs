using TrainComponent.Application.DTOs.Enums;

namespace TrainComponent.Application.DTOs;

public class SortOptionsDto
{
    /// <summary>
    /// Field to sort by. Allowed values: Name, UniqueNumber
    /// </summary>
    public ComponentSortBy? SortBy { get; set; }

    /// <summary>
    /// Sort direction. Allowed values: Asc, Desc
    /// </summary>
    public SortDirection? SortDir { get; set; }
}
