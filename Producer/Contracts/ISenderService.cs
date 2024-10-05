namespace Producer.Contracts;

internal interface ISenderService
{
    bool SendAction(string payload);
}