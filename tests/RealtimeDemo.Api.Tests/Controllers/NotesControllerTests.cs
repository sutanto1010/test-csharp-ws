using Microsoft.AspNetCore.Mvc;
using RealtimeDemo.Api.Contracts;
using RealtimeDemo.Api.Controllers;
using RealtimeDemo.Api.Services;

namespace RealtimeDemo.Api.Tests.Controllers;

public sealed class NotesControllerTests
{
    [Fact]
    public async Task Create_ReturnsCreatedResultAndBroadcastsEvent()
    {
        var service = new InMemoryNoteService();
        var notifier = new FakeRealtimeNotifier();
        var controller = new NotesController(service, notifier);

        var result = await controller.Create(
            new CreateNoteRequest
            {
                Title = "Deployment",
                Content = "Ready for Linux"
            },
            CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payload = Assert.IsType<NoteResponse>(created.Value);

        Assert.Equal(nameof(NotesController.GetById), created.ActionName);
        Assert.Equal("noteCreated", notifier.LastEventName);
        Assert.NotNull(notifier.LastPayload);
        Assert.Equal(payload.Id, notifier.LastPayload!.Id);
    }

    [Fact]
    public void GetById_ReturnsNotFoundWhenNoteDoesNotExist()
    {
        var controller = new NotesController(new InMemoryNoteService(), new FakeRealtimeNotifier());

        var result = controller.GetById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    private sealed class FakeRealtimeNotifier : IRealtimeNotifier
    {
        public string? LastEventName { get; private set; }

        public NoteResponse? LastPayload { get; private set; }

        public Task BroadcastAsync<T>(string eventName, T payload, CancellationToken cancellationToken = default)
        {
            LastEventName = eventName;
            LastPayload = payload as NoteResponse;
            return Task.CompletedTask;
        }
    }
}
