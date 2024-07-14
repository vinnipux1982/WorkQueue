using Consumer;

namespace ConsumerConsole;

internal class Handler : IHandlerMsg
{
    public void Processing(string message)
    {
        Console.WriteLine(message);
        Thread.Sleep(1000);
    }
}