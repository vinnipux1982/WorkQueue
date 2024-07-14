using Producer.Models;

namespace Producer.Contracts;

internal interface IReceiverService
{
    public void ProcessingResult(UserAction userActionResult);
}