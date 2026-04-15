using RealtimeDemo.Api.Contracts;
using RealtimeDemo.Api.Services;

namespace RealtimeDemo.Api.Tests.Services;

public sealed class InMemoryNoteServiceTests
{
    [Fact]
    public void Create_TrimsInputAndStoresNote()
    {
        var service = new InMemoryNoteService();

        var created = service.Create(new CreateNoteRequest
        {
            Title = "  First note  ",
            Content = "  Hello world  "
        });

        var stored = service.GetById(created.Id);

        Assert.NotNull(stored);
        Assert.Equal("First note", stored.Title);
        Assert.Equal("Hello world", stored.Content);
        Assert.Equal(created.Id, stored.Id);
    }

    [Fact]
    public void GetAll_ReturnsNewestNotesFirst()
    {
        var service = new InMemoryNoteService();

        var first = service.Create(new CreateNoteRequest
        {
            Title = "First",
            Content = "One"
        });

        Thread.Sleep(5);

        var second = service.Create(new CreateNoteRequest
        {
            Title = "Second",
            Content = "Two"
        });

        var notes = service.GetAll().ToArray();

        Assert.Equal(2, notes.Length);
        Assert.Equal(second.Id, notes[0].Id);
        Assert.Equal(first.Id, notes[1].Id);
    }
}
