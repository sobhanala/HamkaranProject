
class Program
{
    static void Main(string[] args)
    {

        ProducerLoader producerLoader = new ProducerLoader();

        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string producerDllPath = Path.Combine(basePath, "..", "..", "..", "..","ProdducerDll", "bin", "Debug", "net8.0", "ProducerDll.dll");
        producerDllPath = Path.GetFullPath(producerDllPath);

        Console.WriteLine($"[Main] DLL Path: {producerDllPath}");

        producerLoader.LoadProducer(producerDllPath);


        producerLoader.Start();

        Console.WriteLine("[Main] Producer operation complete. Press any key to exit...");
        Console.ReadKey();
    }
}
