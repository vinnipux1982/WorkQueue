namespace Producer.Contracts;

internal interface IReceiveHandler
{
    void Processing(string message);
}