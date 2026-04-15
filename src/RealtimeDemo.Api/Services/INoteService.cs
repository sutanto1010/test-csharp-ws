using RealtimeDemo.Api.Contracts;
using RealtimeDemo.Api.Models;

namespace RealtimeDemo.Api.Services;

public interface INoteService
{
    IReadOnlyCollection<Note> GetAll();

    Note? GetById(Guid id);

    Note Create(CreateNoteRequest request);
}
