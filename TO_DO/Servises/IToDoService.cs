using TO_DO.DTOs;
using TO_DO.DTOs.Pagination;
using TO_DO.Models;

namespace TO_DO.Servises;

public interface IToDoService
{
    Task<PaginatedListDto<ToDoItemDto>> GetToDoItems(
        int page,
        int pageSize,
        string? search,
        bool? isCompleted);
    Task<ToDoItemDto?> GetToDoItem(int id);
    Task<ToDoItemDto> ChangeTodoItemStatus(int id, bool isCompleted);
    Task<ToDoItemDto> CreateTodoItem(CreateToDoItemRequest request);
}
