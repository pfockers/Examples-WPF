using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Compare_TicketApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TicketDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Tickets")));

// Enums als "low", "in-progress", ... ueber die API (passt zu den TypeScript-Typen).
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower)));

builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(origins)
    .AllowAnyHeader()
    .AllowAnyMethod()));

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key fehlt (z. B. per User-Secrets oder Umgebungsvariable setzen).");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<AuthService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
    db.Database.Migrate();
    if (!db.Tickets.Any())
    {
        var now = DateTime.UtcNow;
        db.Tickets.AddRange(
            new Ticket { Title = "Login-Seite zeigt Fehler 500", Description = "Nach dem Absenden des Formulars erscheint ein Serverfehler.", Priority = Priority.High, Status = Status.Open, CreatedAt = now },
            new Ticket { Title = "Passwort-Reset-Mail anpassen", Description = "Text und Betreff ueberarbeiten.", Priority = Priority.Medium, Status = Status.InProgress, CreatedAt = now },
            new Ticket { Title = "Logo im Footer austauschen", Priority = Priority.Low, Status = Status.Done, CreatedAt = now });
        db.SaveChanges();
    }
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI unter /scalar
}

app.MapPost("/api/auth/login", (LoginRequest req, AuthService auth) =>
    auth.Login(req) is { } result ? Results.Ok(result) : Results.Unauthorized())
    .WithTags("Auth");

var api = app.MapGroup("/api/tickets").RequireAuthorization().WithTags("Tickets");

api.MapGet("/", async (TicketDbContext db) =>
    await db.Tickets.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id)
        .Select(t => TicketDto.From(t)).ToListAsync());

api.MapGet("/{id:int}", async (int id, TicketDbContext db) =>
    await db.Tickets.FindAsync(id) is { } t ? Results.Ok(TicketDto.From(t)) : Results.NotFound());

api.MapPost("/", async (CreateTicketRequest req, TicketDbContext db) =>
{
    var ticket = new Ticket
    {
        Title = req.Title.Trim(),
        Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
        Priority = req.Priority,
        Status = Status.Open,
        CreatedAt = DateTime.UtcNow,
    };
    db.Tickets.Add(ticket);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tickets/{ticket.Id}", TicketDto.From(ticket));
});

api.MapPut("/{id:int}", async (int id, UpdateTicketRequest req, TicketDbContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();
    ticket.Title = req.Title.Trim();
    ticket.Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim();
    ticket.Priority = req.Priority;
    await db.SaveChangesAsync();
    return Results.Ok(TicketDto.From(ticket));
});

api.MapPut("/{id:int}/status", async (int id, UpdateStatusRequest req, TicketDbContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();
    ticket.Status = req.Status;
    await db.SaveChangesAsync();
    return Results.Ok(TicketDto.From(ticket));
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

// Fuer WebApplicationFactory in den Integrationstests.
public partial class Program;
