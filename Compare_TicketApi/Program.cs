using System.Text.Json;
using System.Text.Json.Serialization;
using Compare_TicketApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5300");

builder.Services.AddDbContext<TicketDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Tickets") ?? "Data Source=tickets.db"));

// Enums als "low", "in-progress", ... ueber die API (passt zu den TypeScript-Typen).
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower)));

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins("http://localhost:5173", "http://localhost:4200", "http://localhost:5200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
    db.Database.EnsureCreated();
    if (!db.Tickets.Any())
    {
        var now = DateTime.UtcNow;
        db.Tickets.AddRange(
            new Ticket { Title = "Login-Seite zeigt Fehler 500", Priority = Priority.High, Status = Status.Open, CreatedAt = now },
            new Ticket { Title = "Passwort-Reset-Mail anpassen", Priority = Priority.Medium, Status = Status.InProgress, CreatedAt = now },
            new Ticket { Title = "Logo im Footer austauschen", Priority = Priority.Low, Status = Status.Done, CreatedAt = now });
        db.SaveChanges();
    }
}

app.UseCors();

var api = app.MapGroup("/api/tickets");

api.MapGet("/", async (TicketDbContext db) =>
    await db.Tickets.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id).ToListAsync());

api.MapPost("/", async (CreateTicketRequest req, TicketDbContext db) =>
{
    var title = req.Title?.Trim();
    if (string.IsNullOrEmpty(title) || title.Length > 200)
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["title"] = ["Titel ist erforderlich (max. 200 Zeichen)."] });

    var ticket = new Ticket { Title = title, Priority = req.Priority, Status = Status.Open, CreatedAt = DateTime.UtcNow };
    db.Tickets.Add(ticket);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tickets/{ticket.Id}", ticket);
});

api.MapPut("/{id:int}/status", async (int id, UpdateStatusRequest req, TicketDbContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();
    ticket.Status = req.Status;
    await db.SaveChangesAsync();
    return Results.Ok(ticket);
});

api.MapDelete("/{id:int}", async (int id, TicketDbContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();
    db.Tickets.Remove(ticket);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
