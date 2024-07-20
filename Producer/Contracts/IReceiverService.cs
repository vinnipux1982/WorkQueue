using Producer.Models;

namespace Producer.Contracts;

internal interface IReceiverService
{
    void Init(string hostName, string queueName, Action<string> handler);
}