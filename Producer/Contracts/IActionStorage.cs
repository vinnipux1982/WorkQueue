using Producer.DTOs;

namespace Producer.Contracts;

internal interface IActionStorage
{
    public void Add(ActionInfo action);

    public ActionInfo? GetAction(Guid clientId);
    
}