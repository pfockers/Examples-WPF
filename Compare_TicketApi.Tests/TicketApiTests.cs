using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Compare_TicketApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace Compare_TicketApi.Tests;

public class TicketApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbFile = Path.Combine(Path.GetTempPath(), $"tickets-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Eigene Datenbank pro Testlauf, damit Tests die Entwicklungsdaten nicht beruehren.
        builder.UseSetting("ConnectionStrings:Tickets", $"Data Source={_dbFile}");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        foreach (var f in Directory.GetFiles(Path.GetTempPath(), Path.GetFileName(_dbFile) + "*"))
            File.Delete(f);
    }
}

public class TicketApiTests(TicketApiFactory factory) : IClassFixture<TicketApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower) },
    };

    private async Task<HttpClient> LoggedInClientAsync()
    {
        var client = factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/auth/login", new { username = "demo", password = "demo123" });
        res.EnsureSuccessStatusCode();
        var login = await res.Content.ReadFromJsonAsync<LoginResponse>(Json);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
        return client;
    }

    [Fact]
    public async Task Tickets_without_token_returns_401()
    {
        var res = await factory.CreateClient().GetAsync("/api/tickets");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        var res = await factory.CreateClient().PostAsJsonAsync("/api/auth/login", new { username = "demo", password = "falsch" });
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Create_update_and_delete_ticket()
    {
        var client = await LoggedInClientAsync();

        var created = await client.PostAsJsonAsync("/api/tickets", new { title = "Testticket", description = "Beschreibung", priority = "high" }, Json);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var ticket = (await created.Content.ReadFromJsonAsync<TicketDto>(Json))!;
        Assert.Equal(Status.Open, ticket.Status);
        Assert.Equal(Priority.High, ticket.Priority);

        var statusRes = await client.PutAsJsonAsync($"/api/tickets/{ticket.Id}/status", new { status = "in-progress" }, Json);
        Assert.Equal(Status.InProgress, (await statusRes.Content.ReadFromJsonAsync<TicketDto>(Json))!.Status);

        var updateRes = await client.PutAsJsonAsync($"/api/tickets/{ticket.Id}", new { title = "Neu", description = (string?)null, priority = "low" }, Json);
        var updated = (await updateRes.Content.ReadFromJsonAsync<TicketDto>(Json))!;
        Assert.Equal("Neu", updated.Title);
        Assert.Null(updated.Description);

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/tickets/{ticket.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/tickets/{ticket.Id}")).StatusCode);
    }

    [Fact]
    public async Task Create_with_too_short_title_returns_400()
    {
        var client = await LoggedInClientAsync();
        var res = await client.PostAsJsonAsync("/api/tickets", new { title = "ab", priority = "low" }, Json);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact]
    public async Task List_contains_seed_tickets()
    {
        var client = await LoggedInClientAsync();
        var list = await client.GetFromJsonAsync<List<TicketDto>>("/api/tickets", Json);
        Assert.True(list!.Count >= 3);
    }
}
