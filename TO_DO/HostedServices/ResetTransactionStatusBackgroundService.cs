using Microsoft.EntityFrameworkCore;
using TO_DO.Data;
using TO_DO.Models;

namespace TO_DO.HostedServices;

public class ResetTransactionStatusBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ResetTransactionStatusBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested)
        {
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            var pendingTransactions = await dbContext.Transactions
                .Where(t => t.Status == TransactionStatus.Processing)
                .ToListAsync();
            foreach (var transaction in pendingTransactions)
            {
                transaction.Status = TransactionStatus.Created;
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
