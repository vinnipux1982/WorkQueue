namespace Producer.DTOs;

internal record Message(Guid ClientId, string Payload);