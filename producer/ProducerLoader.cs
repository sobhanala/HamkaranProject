using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using ProducerInterface;
using LoggerLib;

public class ProducerLoader
{
    private IProducer _producer;
    private int _threadCount;
    private int _retryCount;
    private int _messageIntervalMs;
    private readonly HttpClient _httpClient = new();
    private readonly string _apiUrl = "https://localhost:5000/api/produce";

    private static readonly ConcurrentQueue<string> _messageQueue = new();
    private LogService<ProducerLoader> _logger = LogService<ProducerLoader>.Instance;

    public void LoadProducer(string dllPath)
    {
        if (!File.Exists(dllPath))
        {
            _logger.LogWarning("Producer DLL not found.");
            return;
        }

        try
        {
            Assembly assembly = Assembly.LoadFile(dllPath);
            Type? producerType = assembly.GetTypes()
              .FirstOrDefault(t => typeof(IProducer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (producerType == null)
            {
                _logger.LogError("ProducerPlugin class not found.");
                return;
            }

            LoadAttribute(producerType);
            _producer = Activator.CreateInstance(type: producerType) as IProducer;
            _logger.LogInformation($"Producer loaded with {_threadCount} threads, {_retryCount} retries, and {_messageIntervalMs}ms interval.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading producer: {ex.Message}");
        }
    }

    private void LoadAttribute(Type producerType)
    {
        var attribute = Attribute.GetCustomAttribute(producerType,
                                                     typeof(ProducerSettingsAttribute)) as ProducerSettingsAttribute;

        _threadCount = attribute?.ThreadCount ?? 3;
        _retryCount = attribute?.RetryCount ?? 5;
        _messageIntervalMs = attribute?.MessageIntervalMs ?? 2000;
    }

    public void Start()
    {
        if (_producer != null)
        {
            _logger.LogInformation($"Starting {_threadCount} producer threads...");

            for (int i = 0; i < _threadCount; i++)
            {
                int threadId = i + 1;
                Task.Run(() => ProduceMessages(threadId));
            }

            Task.Run(() => ProcessQueue());
        }
        else
        {
            _logger.LogError("No producer instance available.");
        }
    }

    private async Task ProduceMessages(int threadId)
    {
        while (true)
        {
            try
            {
                string message = _producer.ProduceMessage();
                if (!string.IsNullOrEmpty(message))
                {
                    _messageQueue.Enqueue(message);
                    _logger.LogInformation($"[Thread {threadId}] Produced: {message}");
                }
                else
                {
                    _logger.LogWarning($"[Thread {threadId}] Produced an empty message");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Thread {threadId}] Error producing message: {ex.Message}");
            }

            await Task.Delay(_messageIntervalMs);
        }
    }

    private async Task ProcessQueue()
    {
        while (true)
        {
            try
            {
                if (_messageQueue.TryDequeue(out var message))
                {
                    await SendMessageAsync(message);
                }
                else
                {
                    _logger.LogDebug("Queue is empty, waiting before retry.");
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing queue: {ex.Message}");
            }
        }
    }


    private async Task SendMessageAsync(string message)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Message sent successfully: {message}");
            }
            else
            {
                _messageQueue.Enqueue(message);
                _logger.LogWarning($"Failed to send message. Retrying later: {message}");
            }
        }
        catch (Exception ex)
        {
            _messageQueue.Enqueue(message);
            _logger.LogError($"Error sending message: {ex.Message}");
        }
 
    }
}
