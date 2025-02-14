using System;

namespace MessageBrokerAPI.Services;

public class BrokerService
{
    private readonly MessageQueue _queue;

    public BrokerService(MessageQueue queue)
    {
        _queue = queue;
    }

    public void Produce(string message)
    {
        _queue.Enqueue(message);
        Console.WriteLine($"Produced: {message}");
    }

    public string Consume()
    {
        var message = _queue.Dequeue();
        if (message != null)
        {
            Console.WriteLine($"Consumed: {message}");
        }
        return message ?? "No messages available";
    }
}
