using IConsumernamespace;
using System.Reflection;
using System.Text.Json;
using System.Text;

public class ConsumerPlugin : IConsumer
{
    private static readonly string SavePath = "consumed_messages.txt";

    public async Task ProcessMessageAsync(string serializedMessage)
    {
        try
        {
            var messageInstance = JsonSerializer.Deserialize<Dictionary<string, object>>(serializedMessage);
            if (messageInstance != null)
            {
                await SaveMessageAsync(messageInstance);
            }
        }
        catch (JsonException ex)
        {
            throw new Exception("Failed to deserialize message.", ex);
        }
    }

    private async Task SaveMessageAsync(Dictionary<string, object> messageInstance)
    {
        var logEntry = new StringBuilder();
        logEntry.Append($"{DateTime.UtcNow} ");

        foreach (var kvp in messageInstance)
        {
            logEntry.Append($"{kvp.Key}={kvp.Value} ");
        }

        logEntry.Append(Environment.NewLine);
        await File.AppendAllTextAsync(SavePath, logEntry.ToString());
    }
}
