using Microsoft.EntityFrameworkCore;
using TO_DO.Data;
using TO_DO.DTOs;
using TO_DO.DTOs.Pagination;
using TO_DO.Models;

namespace TO_DO.Servises;

public class ToDoService : IToDoService
{
    private readonly ToDoDbContext _dbContext;
    private readonly IEmailSender _emailSender;

    public ToDoService(ToDoDbContext dbContext, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _emailSender = emailSender;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isCompletded"></param>
    /// <returns></returns>
    public async Task<ToDoItemDto?> ChangeTodoItemStatus(string userId, int id, bool isCompletded)
    {
        var item = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (item is null)
        {
            return null;
        }
        item.IsCompleted = isCompletded;
        item.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return new ToDoItemDto
        {
            Id = item.Id,
            Text = item.Text,
            CreatedAt = item.CreatedAt,
            Iscompleted = isCompletded
        };
    }

    public async Task<ToDoItemDto> CreateTodoItem(string userId, CreateToDoItemRequest request)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException();
        }

        var now = DateTime.UtcNow;
        var item = new ToDoItem
        {
            Text = request.Text,
            CreatedAt = now,
            UpdatedAt = now,
            IsCompleted = false,
            UserId = userId
        };

        item = _dbContext.ToDoItems.Add(item).Entity;

        await _dbContext.SaveChangesAsync();

        await _emailSender.SendEmail(user.Email, "New ToDo Added", item.Text);

        return new ToDoItemDto
        {
            Id = item.Id,
            Text = item.Text,
            Iscompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt

        };
    }

    public async Task<ToDoItemDto?> GetToDoItem(string userId, int id)
    {
        var item = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
      
        return item is not null?
        new ToDoItemDto
        {
            Id = item.Id,
            Text = item.Text,
            CreatedAt = item.CreatedAt,
            Iscompleted = item.IsCompleted
        }:null;
    }

    public async Task<PaginatedListDto<ToDoItemDto>> GetToDoItems(
        string userId,
        int page,
        int pageSize, 
        string? search,
        bool? isCompleted)
    {
        IQueryable<ToDoItem> query = _dbContext.ToDoItems.Where(x=>x.UserId ==  userId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Text.Contains(search));
        }
        if (isCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == isCompleted);
        }
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        return new PaginatedListDto<ToDoItemDto>(
            items.Select(t=> new ToDoItemDto
            {
                Id=t.Id,
                Text=t.Text,
                CreatedAt=t.CreatedAt,
                Iscompleted=t.IsCompleted
            }),
            new PaginationMeta(page, pageSize, totalCount)
            );
    }
}
