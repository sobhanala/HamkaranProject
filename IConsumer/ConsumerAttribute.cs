namespace IConsumernamespace;

[AttributeUsage(AttributeTargets.Class)]
public class ConsumerAttribute : Attribute
{
    public int ThreadCount { get; set; }
    public int RetryCount { get; set; }

    public ConsumerAttribute(int threadCount, int retrycount)
    {
        ThreadCount = threadCount;
        RetryCount = retrycount;
    }

}