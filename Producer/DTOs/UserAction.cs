namespace Producer.DTOs;

public record UserAction(Guid ClientId, string Payload);