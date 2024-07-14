namespace Producer.Models;

public record UserAction(Guid ClientId, string Payload);