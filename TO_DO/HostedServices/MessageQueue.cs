using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Concurrent;
using TO_DO.Data;
using TO_DO.Models;

namespace TO_DO.HostedServices;

public class MessageQueue
{
    //private readonly ConcurrentQueue<CreateTransactionRequest> _queue = new();
    private readonly IServiceProvider _serviceProvider;

    public MessageQueue(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async void Enqueue(CreateTransactionRequest request)
    {
        //_queue.Enqueue(request);
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        dbContext.Transactions.Add(new Transaction{
            Data = request.Data,
            Status = TransactionStatus.Created
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task<Transaction?> Dequeue()
    {
        //_queue.TryDequeue(out var request);
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        var transaction = await dbContext.Transactions
            .Where(t => t.Status == TransactionStatus.Created)
            .OrderBy(t => t.Id)
            .FirstOrDefaultAsync();
        transaction.Status = TransactionStatus.Processing;
        Log.Fatal($"Transaction Data {transaction.Data} - Transaction Status {transaction.Status}");
        await dbContext.SaveChangesAsync();
        return transaction;
    }

    public async Task Acknowlage(int id)
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Status == TransactionStatus.Processing && t.Id == id);
        if (transaction is null)
        {
            return;
        }
        transaction.Status = TransactionStatus.Processed;
        Log.Fatal($"Transaction Data {transaction.Data} - Transaction Status {transaction.Status}");
        await dbContext.SaveChangesAsync();
    }

    public async Task Abort(int id)
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Status == TransactionStatus.Processing && t.Id == id);
        if (transaction is null)
        {
            return;
        }
        transaction.Status = TransactionStatus.Aborted;
        Log.Fatal($"Transaction Data {transaction.Data} - Transaction Status {transaction.Status}");
        await dbContext.SaveChangesAsync();
    }
}
