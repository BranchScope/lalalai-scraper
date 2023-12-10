namespace lalalai;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        await Worker.Beast();
    }
}