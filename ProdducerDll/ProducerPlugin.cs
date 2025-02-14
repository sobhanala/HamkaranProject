using ProducerInterface;

using System.Text.Json;


[ProducerSettings(3, 5, 1000)]
public class ProducerPlugin : IProducer
{
    private static int _messageCounter = 1;  
    private string message = "Hello from ProducerPlugin!";



    public string ProduceMessage()
    {
        
        _messageCounter++;
        return JsonSerializer.Serialize(new {_messageCounter, message}); 
    }


}
