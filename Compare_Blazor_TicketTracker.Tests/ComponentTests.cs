using Bunit;
using Compare_Blazor_TicketTracker.Components;
using Compare_Blazor_TicketTracker.Models;
using Microsoft.AspNetCore.Components;

namespace Compare_Blazor_TicketTracker.Tests;

public class TicketFormTests : BunitContext
{
    [Fact]
    public void Short_title_shows_validation_message_and_does_not_invoke_callback()
    {
        TicketInput? received = null;
        var cut = Render<TicketForm>(p => p.Add(c => c.OnAdd,
            EventCallback.Factory.Create<TicketInput>(this, (TicketInput i) => received = i)));

        cut.Find("input").Change("ab");
        cut.Find("form").Submit();

        Assert.Contains("zwischen 3 und 200", cut.Find(".validation-message").TextContent);
        Assert.Null(received);
    }

    [Fact]
    public void Valid_input_invokes_callback_with_title_and_priority()
    {
        TicketInput? received = null;
        var cut = Render<TicketForm>(p => p.Add(c => c.OnAdd,
            EventCallback.Factory.Create<TicketInput>(this, (TicketInput i) => received = new TicketInput { Title = i.Title, Priority = i.Priority })));

        cut.Find("input").Change("Drucker defekt");
        cut.Find("select").Change("High");
        cut.Find("form").Submit();

        Assert.NotNull(received);
        Assert.Equal("Drucker defekt", received!.Title);
        Assert.Equal(Priority.High, received.Priority);
    }
}

public class TicketItemTests : BunitContext
{
    private static readonly Ticket Sample = new(7, "Login-Seite zeigt Fehler", null, Priority.High, Status.Open, new DateTime(2026, 10, 9, 10, 0, 0, DateTimeKind.Utc));

    [Fact]
    public void Title_links_to_detail_page()
    {
        var cut = Render<TicketItem>(p => p.Add(c => c.Ticket, Sample));

        Assert.Equal("tickets/7", cut.Find("a.title").GetAttribute("href"));
        Assert.Equal(Sample.Title, cut.Find("a.title").TextContent);
    }

    [Fact]
    public void Status_change_and_delete_are_reported_to_the_caller()
    {
        Status? newStatus = null;
        var deleted = false;
        var cut = Render<TicketItem>(p => p
            .Add(c => c.Ticket, Sample)
            .Add(c => c.OnStatusChange, EventCallback.Factory.Create<Status>(this, (Status s) => newStatus = s))
            .Add(c => c.OnDelete, EventCallback.Factory.Create(this, () => deleted = true)));

        cut.Find("select").Change("Done");
        cut.Find("button.danger").Click();

        Assert.Equal(Status.Done, newStatus);
        Assert.True(deleted);
    }
}
