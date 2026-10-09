using System.ComponentModel.DataAnnotations;

namespace Compare_Blazor_TicketTracker.Models;

public enum Priority { Low, Medium, High }

public enum Status { Open, InProgress, Done }

public record Ticket(int Id, string Title, string? Description, Priority Priority, Status Status, DateTime CreatedAt);

// DataAnnotations: EditForm validiert Eingaben anhand dieser Attribute (Gegenstueck zu Reactive Forms bzw. Hand-Validierung in React).
public class TicketInput
{
    [Required(ErrorMessage = "Titel ist erforderlich.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Titel muss zwischen 3 und 200 Zeichen lang sein.")]
    public string Title { get; set; } = "";

    [StringLength(1000, ErrorMessage = "Beschreibung darf hoechstens 1000 Zeichen lang sein.")]
    public string? Description { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;
}

public class LoginInput
{
    [Required(ErrorMessage = "Benutzername ist erforderlich.")]
    public string Username { get; set; } = "demo";

    [Required(ErrorMessage = "Passwort ist erforderlich.")]
    public string Password { get; set; } = "";
}

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
