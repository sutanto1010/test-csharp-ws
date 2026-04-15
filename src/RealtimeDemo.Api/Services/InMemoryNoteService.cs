using RealtimeDemo.Api.Contracts;
using RealtimeDemo.Api.Models;

namespace RealtimeDemo.Api.Services;

public sealed class InMemoryNoteService : INoteService
{
    private readonly object _syncRoot = new();
    private readonly List<Note> _notes = [];

    public IReadOnlyCollection<Note> GetAll()
    {
        lock (_syncRoot)
        {
            return _notes
                .OrderByDescending(note => note.CreatedAtUtc)
                .ToArray();
        }
    }

    public Note? GetById(Guid id)
    {
        lock (_syncRoot)
        {
            return _notes.FirstOrDefault(note => note.Id == id);
        }
    }

    public Note Create(CreateNoteRequest request)
    {
        var note = new Note(
            Guid.NewGuid(),
            request.Title.Trim(),
            request.Content.Trim(),
            DateTimeOffset.UtcNow);

        lock (_syncRoot)
        {
            _notes.Add(note);
        }

        return note;
    }
}
