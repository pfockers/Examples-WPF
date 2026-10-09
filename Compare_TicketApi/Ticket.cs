using System.ComponentModel.DataAnnotations;

namespace Compare_TicketApi;

public enum Priority { Low, Medium, High }

public enum Status { Open, InProgress, Done }

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Die API gibt DTOs zurueck, nicht die Datenbank-Entitaet.
public record TicketDto(int Id, string Title, string? Description, Priority Priority, Status Status, DateTime CreatedAt)
{
    public static TicketDto From(Ticket t) => new(t.Id, t.Title, t.Description, t.Priority, t.Status, t.CreatedAt);
}

public record CreateTicketRequest(
    [property: Required, StringLength(200, MinimumLength = 3)] string Title,
    [property: StringLength(1000)] string? Description,
    Priority Priority);

public record UpdateTicketRequest(
    [property: Required, StringLength(200, MinimumLength = 3)] string Title,
    [property: StringLength(1000)] string? Description,
    Priority Priority);

public record UpdateStatusRequest(Status Status);

public record LoginRequest(
    [property: Required] string Username,
    [property: Required] string Password);

public record LoginResponse(string Token, string Username, DateTime ExpiresAt);
