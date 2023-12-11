namespace lalalai;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("trying to...");
        Worker.SetDefaultWebFirstAssertionTimeout(40000);
        var urls = await Worker.Beast();
        Console.WriteLine($"Vocals: {urls["vocals"]}");
        Console.WriteLine($"Instrumental: {urls["instrumental"]}");
    }
}