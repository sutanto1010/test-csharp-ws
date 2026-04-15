using Microsoft.AspNetCore.Mvc;
using RealtimeDemo.Api.Contracts;
using RealtimeDemo.Api.Services;

namespace RealtimeDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotesController(INoteService noteService, IRealtimeNotifier realtimeNotifier) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<NoteResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<NoteResponse>> GetAll()
    {
        var notes = noteService.GetAll()
            .Select(NoteResponse.FromModel)
            .ToArray();

        return Ok(notes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<NoteResponse> GetById(Guid id)
    {
        var note = noteService.GetById(id);

        return note is null
            ? NotFound()
            : Ok(NoteResponse.FromModel(note));
    }

    [HttpPost]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NoteResponse>> Create(
        [FromBody] CreateNoteRequest request,
        CancellationToken cancellationToken)
    {
        var createdNote = noteService.Create(request);
        var response = NoteResponse.FromModel(createdNote);

        await realtimeNotifier.BroadcastAsync("noteCreated", response, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}
