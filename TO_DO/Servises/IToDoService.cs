using TO_DO.DTOs;
using TO_DO.DTOs.Pagination;
using TO_DO.Models;

namespace TO_DO.Servises;

public interface IToDoService
{
    Task<PaginatedListDto<ToDoItemDto>> GetToDoItems(
        string userId,
        int page,
        int pageSize,
        string? search,
        bool? isCompleted);
    Task<ToDoItemDto?> GetToDoItem(string userId, int id);
    Task<ToDoItemDto> ChangeTodoItemStatus(string userId, int id, bool isCompleted);
    Task<ToDoItemDto> CreateTodoItem(string userId, CreateToDoItemRequest request);
}
