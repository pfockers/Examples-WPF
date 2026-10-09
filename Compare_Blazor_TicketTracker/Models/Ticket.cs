namespace Compare_Blazor_TicketTracker.Models;

public enum Priority { Low, Medium, High }

public enum Status { Open, InProgress, Done }

public record Ticket(int Id, string Title, Priority Priority, Status Status, DateTime CreatedAt);

public static class TicketLabels
{
    public static readonly Status[] Statuses = Enum.GetValues<Status>();
    public static readonly Priority[] Priorities = Enum.GetValues<Priority>();

    public static string Label(this Status s) => s switch
    {
        Status.Open => "Offen",
        Status.InProgress => "In Arbeit",
        _ => "Erledigt",
    };

    public static string Label(this Priority p) => p switch
    {
        Priority.Low => "Niedrig",
        Priority.Medium => "Mittel",
        _ => "Hoch",
    };

    // CSS-Klassen sind klein geschrieben bzw. mit Bindestrich.
    public static string Css(this Status s) => s == Status.InProgress ? "in-progress" : s.ToString().ToLowerInvariant();

    public static string Css(this Priority p) => p.ToString().ToLowerInvariant();
}
