namespace Producer.Contracts;

internal interface ISenderService
{
    Task SendAction(string payload);
}