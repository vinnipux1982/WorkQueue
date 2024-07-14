namespace Producer.Models;

internal record ActionInfo(TaskCompletionSource<string?> TaskCompletionSource, string Payload, Guid ClientId);