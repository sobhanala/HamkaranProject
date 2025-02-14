
namespace ProducerInterface;

[AttributeUsage(AttributeTargets.Class)]
public class ProducerSettingsAttribute : Attribute
{
    public int ThreadCount { get; }
    public int RetryCount { get; }
    public int MessageIntervalMs { get; }

    public ProducerSettingsAttribute(int threadCount, int retryCount, int messageIntervalMs)
    {
        ThreadCount = threadCount;
        RetryCount = retryCount;
        MessageIntervalMs = messageIntervalMs;
    }
}
