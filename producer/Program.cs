
class Program
{
    static void Main(string[] args)
    {

        ProducerLoader producerLoader = new ProducerLoader();

        producerLoader.LoadProducer("C:\\Users\\Click\\source\\repos\\hamkaran project\\ProdducerDll\\bin\\Debug\\net8.0\\ProducerDll.dll");


        producerLoader.Start();

        Console.WriteLine("[Main] Producer operation complete. Press any key to exit...");
        Console.ReadKey();
    }
}
