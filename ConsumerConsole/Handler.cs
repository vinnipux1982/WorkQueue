using System.Text.Json;
using Consumer;
using ConsumerConsole.DTOs;

namespace ConsumerConsole;

internal class Handler : IHandlerMsg
{
    public string Processing(string message)
    {
        Console.WriteLine(message);
        var messageDto = JsonSerializer.Deserialize<MessageDto>(message);
        Thread.Sleep(1000);

        return JsonSerializer.Serialize(new MessageDto(messageDto!.ClientId, messageDto.Payload + " Receive"));
    }
}
