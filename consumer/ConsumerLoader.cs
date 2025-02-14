using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using IConsumernamespace;
using LoggerLib;

public class ConsumerLoader
{
    private IConsumer? _consumerInstance;
    private int _threadCount;
    private int _retryCount;
    private int _messageIntervalMs;
    private bool _isRunning;
    private readonly HttpClient _httpClient = new();
    private readonly string _apiUrl = "https://localhost:5000/api/consume";
    private LogService<ConsumerLoader> _logger = LogService<ConsumerLoader>.Instance;

    public void LoadConsumer(string dllPath)
    {
        if (!File.Exists(dllPath))
        {
            _logger.LogWarning("Consumer DLL not found.");
            return;
        }

        try
        {
            Assembly assembly = Assembly.LoadFile(dllPath);
            Type? consumerType = assembly.GetTypes()
               .FirstOrDefault(t => typeof(IConsumer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (consumerType == null)
            {
                _logger.LogError("No class implementing IConsumer found.");
                return;
            }

            _consumerInstance = Activator.CreateInstance(consumerType) as IConsumer;
            LoadSettings(consumerType);

            _logger.LogInformation($"Consumer loaded with {_threadCount} threads, {_retryCount} retries, and {_messageIntervalMs}ms interval.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading consumer: {ex.Message}");
        }
    }

    private void LoadSettings(Type consumerType)
    {
        var attribute = Attribute.GetCustomAttribute(consumerType, typeof(ConsumerAttribute)) as ConsumerAttribute;

        _threadCount = attribute?.ThreadCount ?? 3;
        _retryCount = attribute?.RetryCount ?? 5;
        _messageIntervalMs = 2000;
    }

    public void Start()
    {
        if (_consumerInstance == null)
        {
            _logger.LogError("Consumer is not loaded properly.");
            return;
        }

        _logger.LogInformation("Starting consumer...");
        _isRunning = true;

        for (int i = 0; i < _threadCount; i++)
        {
            int threadId = i + 1;
            Task.Run(() => ConsumeLoop(threadId));
        }
    }

    private async Task ConsumeLoop(int threadId)
    {
        while (_isRunning)
        {
            for (int i = 0; i < _retryCount; i++)
            {
                try
                {
                    string serializedMessage = await FetchMessageFromApiAsync();

                    if (!string.IsNullOrEmpty(serializedMessage))
                    {
                        await _consumerInstance.ProcessMessageAsync(serializedMessage);
                        _logger.LogInformation($"[Thread {threadId}] Message processed successfully.");
                    }
                    else
                    {
                        _logger.LogWarning($"[Thread {threadId}] No message received from the API.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[Thread {threadId}] Error processing message: {ex.Message}");
                }
            }

            _logger.LogInformation($"[Thread {threadId}] Waiting {_messageIntervalMs / 1000} seconds...");
            await Task.Delay(_messageIntervalMs);
        }
    }

    private async Task<string> FetchMessageFromApiAsync()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return responseString;

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching message from API: {ex.Message}");
            return null;
        }
    }


}