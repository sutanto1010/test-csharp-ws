using RealtimeDemo.Api.Models;

namespace RealtimeDemo.Api.Contracts;

public sealed record NoteResponse(
    Guid Id,
    string Title,
    string Content,
    DateTimeOffset CreatedAtUtc)
{
    public static NoteResponse FromModel(Note note) =>
        new(note.Id, note.Title, note.Content, note.CreatedAtUtc);
}
