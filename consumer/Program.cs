class Program
{
    static void Main(string[] args)
    {

        ConsumerLoader consumerloader = new ConsumerLoader();

        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string producerDllPath = Path.Combine(basePath, "..", "..", "..", "..", "ConsumerDll", "bin", "Debug", "net8.0", "ConsumerDll.dll");
        producerDllPath = Path.GetFullPath(producerDllPath);

        Console.WriteLine($"[Main] DLL Path: {producerDllPath}");

        consumerloader.LoadConsumer(producerDllPath);


        consumerloader.Start();

        Console.WriteLine("[Main] Consumer operation complete.");
        Console.ReadKey();
    }
}
