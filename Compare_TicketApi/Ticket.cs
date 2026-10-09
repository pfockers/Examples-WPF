namespace Compare_TicketApi;

public enum Priority { Low, Medium, High }

public enum Status { Open, InProgress, Done }

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CreateTicketRequest(string Title, Priority Priority);

public record UpdateStatusRequest(Status Status);
