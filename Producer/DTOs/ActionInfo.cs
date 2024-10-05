namespace Producer.DTOs;

internal record ActionInfo(TaskCompletionSource<string?> TaskCompletionSource, string Payload, Guid ClientId, DateTimeOffset StartDate);