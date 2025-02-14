namespace IConsumernamespace;
public interface IConsumer
{
    Task ProcessMessageAsync(string serializedMessage);

}
