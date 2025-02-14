class Program
{
    static void Main(string[] args)
    {

        ConsumerLoader consumerloader = new ConsumerLoader();

        consumerloader.LoadConsumer("C:\\Users\\Click\\source\\repos\\hamkaran project\\ConsumerDll\\bin\\Debug\\net8.0\\ConsumerDll.dll");


        consumerloader.Start();

        Console.WriteLine("[Main] Consumer operation complete.");
        Console.ReadKey();
    }
}
