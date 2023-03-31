using Microsoft.EntityFrameworkCore;
using TO_DO.Models;
namespace TO_DO.Data;

public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
}
