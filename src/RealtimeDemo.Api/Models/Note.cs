namespace RealtimeDemo.Api.Models;

public sealed record Note(
    Guid Id,
    string Title,
    string Content,
    DateTimeOffset CreatedAtUtc);
