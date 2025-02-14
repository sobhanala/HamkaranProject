using LoggerLib;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace MessageBrokerAPI.Services;

public class MessageQueue
{
    private readonly ConcurrentQueue<string> queue = new();
    private readonly string filePath = "messages.log";
    private readonly object fileLock = new();
    private LogService<MessageQueue> _logger = LogService<MessageQueue>.Instance;

    public MessageQueue()
    {
        EnsureFileExists();
        LoadMessagesFromFile();
    }

    public void Enqueue(string message)
    {
        queue.Enqueue(message);
        AppendMessageToFile(message);
    }

    public string Dequeue()
    {
        if (queue.TryDequeue(out var message))
        {
            DeleteFirstMessageFromFile();
            return message;
        }
        else
        {
            return ReadFromFileAndRemove();
        }
    }

    private void AppendMessageToFile(string message)
    {
        lock (fileLock)
        {
            try
            {
                EnsureFileExists();
                File.AppendAllText(filePath, message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot Append Message To File", ex);
            }
        }
    }

    private void LoadMessagesFromFile()
    {
        lock (fileLock)
        {
            EnsureFileExists();
            var messages = File.ReadAllLines(filePath);
            foreach (var msg in messages)
            {
                queue.Enqueue(msg);
            }
        }
    }

    private string ReadFromFileAndRemove()
    {
        lock (fileLock)
        {
            EnsureFileExists();

            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length == 0) return null;

                string firstMessage = lines[0];
                File.WriteAllLines(filePath, lines[1..]);

                return firstMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to read from file", ex);
            }

            return null;
        }
    }

    private void DeleteFirstMessageFromFile()
    {
        lock (fileLock)
        {
            EnsureFileExists();

            try
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length > 0)
                {
                    File.WriteAllLines(filePath, lines[1..]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete from file", ex);
            }
        }
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(filePath))
        {
            try
            {
                File.Create(filePath).Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create message log file", ex);
            }
        }
    }
}
