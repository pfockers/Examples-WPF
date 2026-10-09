using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Compare_Blazor_TicketTracker.Models;

namespace Compare_Blazor_TicketTracker.Services;

// Service: Zustand und Logik liegen ausserhalb der Komponenten und werden per Dependency Injection geteilt.
public class TicketService(HttpClient http)
{
    private const string Base = "http://localhost:5300/api/tickets";

    // Enums als "low", "in-progress", ... (wie in der API).
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower) },
    };

    private List<Ticket> _tickets = [];

    public string Error { get; private set; } = "";
    public string Search { get; set; } = "";
    public string Filter { get; set; } = "all";

    // Blazor hat kein useMemo/computed: die Property wird bei jedem Rendern neu berechnet.
    public IEnumerable<Ticket> Visible => _tickets.Where(t =>
        (Filter == "all" || t.Status.ToString() == Filter) &&
        t.Title.Contains(Search, StringComparison.OrdinalIgnoreCase));

    public int VisibleCount => Visible.Count();

    public int Count(Status status) => _tickets.Count(t => t.Status == status);

    public async Task LoadAsync()
    {
        try
        {
            _tickets = await http.GetFromJsonAsync<List<Ticket>>(Base, Json) ?? [];
        }
        catch (HttpRequestException)
        {
            Error = "Backend nicht erreichbar (http://localhost:5300).";
        }
    }

    public async Task AddAsync(string title, Priority priority)
    {
        var res = await http.PostAsJsonAsync(Base, new { title, priority }, Json);
        res.EnsureSuccessStatusCode();
        _tickets.Insert(0, (await res.Content.ReadFromJsonAsync<Ticket>(Json))!);
    }

    public async Task ChangeStatusAsync(int id, Status status)
    {
        var res = await http.PutAsJsonAsync($"{Base}/{id}/status", new { status }, Json);
        res.EnsureSuccessStatusCode();
        var updated = (await res.Content.ReadFromJsonAsync<Ticket>(Json))!;
        _tickets[_tickets.FindIndex(t => t.Id == id)] = updated;
    }

    public async Task RemoveAsync(int id)
    {
        (await http.DeleteAsync($"{Base}/{id}")).EnsureSuccessStatusCode();
        _tickets.RemoveAll(t => t.Id == id);
    }
}
