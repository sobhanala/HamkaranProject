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
            var messageInstance = JsonSerializer.Deserialize<JsonElement>(serializedMessage);
            if (messageInstance.ValueKind == JsonValueKind.Object)
            {
                var logEntry = new StringBuilder();
                logEntry.Append($"{DateTime.UtcNow} ");

                foreach (var property in messageInstance.EnumerateObject())
                {
                    logEntry.Append($"{property.Name}={property.Value} ");
                }

                logEntry.Append(Environment.NewLine);
                await File.AppendAllTextAsync(SavePath, logEntry.ToString());
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[Error] Failed to deserialize message: {ex.Message}");
            Console.WriteLine(serializedMessage);
        }
    }
}
