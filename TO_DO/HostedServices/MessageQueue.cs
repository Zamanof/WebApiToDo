using System.Collections.Concurrent;

namespace TO_DO.HostedServices;

public class MessageQueue
{
    private readonly ConcurrentQueue<CreateTransactionRequest> _queue = new();

    public void Enqueue(CreateTransactionRequest request)
    {
        _queue.Enqueue(request);
    }

    public CreateTransactionRequest Dequeue()
    {
        _queue.TryDequeue(out var request);
        return request;
    }

}
