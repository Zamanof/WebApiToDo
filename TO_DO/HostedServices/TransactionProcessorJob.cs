using Serilog;
using TO_DO.Data;

namespace TO_DO.HostedServices
{
    public class TransactionProcessorJob : IHostedService
    {
        private MessageQueue _messageQueue;

        public TransactionProcessorJob(MessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }
        private bool _run;

        private async Task Run()
        {
            while (_run)
            {
                var message = _messageQueue.Dequeue();
                if (message is not null)
                {
                    Log.Fatal("Transaction {Data} Started", message.Data);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    Log.Fatal("Transaction {Data} Finished", message.Data);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
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
}
