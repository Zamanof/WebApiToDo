using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TO_DO.Models;
namespace TO_DO.Data;

public class ToDoDbContext : IdentityDbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
    {
    }
    public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
}
