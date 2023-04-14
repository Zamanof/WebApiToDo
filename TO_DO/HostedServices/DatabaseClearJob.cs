using Microsoft.EntityFrameworkCore;
using System.Text;
using TO_DO.Data;

namespace TO_DO.HostedServices;

public class DatabaseClearJob : IHostedService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _provider;
 

    public DatabaseClearJob(ILogger<DatabaseClearJob> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    private bool _run;


    private async Task Run()
    {
        while (_run)
        {
            var scope = _provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            await Task.Delay(TimeSpan.FromSeconds(30));
            _logger.LogError("Transaction Processor Service is running");
            _logger.LogCritical($"Todos count = {dbContext.ToDoItems.Count()}");
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {


        _run = true;
        Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _run = false;
        return Task.CompletedTask;
    }
}
