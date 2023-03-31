using Microsoft.AspNetCore.Mvc;

namespace TO_DO.DTOs;

public class ToDoQueryFilters
{
    /// <summary>
    /// Search by Text
    /// </summary>
    [FromQuery(Name = "search")]
    public string? Search { get; set; }
    
    /// <summary>
    /// Id ToDo item is completed or no
    /// </summary>
    [FromQuery(Name ="completed")]
    public bool? IsCompleted { get; set; }
}
