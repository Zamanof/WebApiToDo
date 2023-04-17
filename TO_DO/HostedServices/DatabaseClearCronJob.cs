using Quartz;
using TO_DO.Data;

namespace TO_DO.HostedServices;

public class DatabaseClearCronJob : IJob
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _provider;


    public DatabaseClearCronJob(ILogger<DatabaseClearJob> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }
       

    public async Task Execute(IJobExecutionContext context)
    {
        var scope = _provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        _logger.LogError("Transaction Processor Service is running");
        _logger.LogCritical($"Todos count = {dbContext.ToDoItems.Count()}");
    }
}
