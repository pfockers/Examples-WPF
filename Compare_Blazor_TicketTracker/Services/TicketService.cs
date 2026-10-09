using System.Net.Http.Json;
using System.Text.Json;
using Compare_Blazor_TicketTracker.Models;

namespace Compare_Blazor_TicketTracker.Services;

// Service: Zustand und Logik liegen ausserhalb der Komponenten und werden per Dependency Injection geteilt.
public class TicketService(IHttpClientFactory factory)
{
    private List<Ticket> _tickets = [];

    public bool Loading { get; private set; }
    public string LoadError { get; private set; } = "";
    public string ActionError { get; private set; } = "";
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
        Loading = true;
        LoadError = "";
        try
        {
            _tickets = await SendAsync<List<Ticket>>(HttpMethod.Get, "api/tickets") ?? [];
        }
        catch (ApiException e)
        {
            LoadError = e.Message;
        }
        finally
        {
            Loading = false;
        }
    }

    public Task<Ticket?> GetAsync(int id) => SendAsync<Ticket>(HttpMethod.Get, $"api/tickets/{id}");

    public Task<Ticket?> UpdateAsync(int id, TicketInput input) => SendAsync<Ticket>(HttpMethod.Put, $"api/tickets/{id}", input);

    // Fuer die Detailseite: loescht, ohne die Liste zu veraendern (die wird beim Zurueckgehen neu geladen).
    public Task DeleteByIdAsync(int id) => SendAsync<object>(HttpMethod.Delete, $"api/tickets/{id}");

    public Task AddAsync(TicketInput input) => ActionAsync(async () =>
    {
        var created = await SendAsync<Ticket>(HttpMethod.Post, "api/tickets", input);
        _tickets.Insert(0, created!);
    });

    public Task ChangeStatusAsync(int id, Status status) => ActionAsync(async () =>
    {
        var updated = await SendAsync<Ticket>(HttpMethod.Put, $"api/tickets/{id}/status", new { status });
        _tickets[_tickets.FindIndex(t => t.Id == id)] = updated!;
    });

    public Task RemoveAsync(int id) => ActionAsync(async () =>
    {
        await SendAsync<object>(HttpMethod.Delete, $"api/tickets/{id}");
        _tickets.RemoveAll(t => t.Id == id);
    });

    // Gemeinsamer Wrapper: Fehler anzeigen, statt sie zu verschlucken.
    private async Task ActionAsync(Func<Task> action)
    {
        ActionError = "";
        try
        {
            await action();
        }
        catch (ApiException e)
        {
            ActionError = e.Message;
        }
    }

    private async Task<T?> SendAsync<T>(HttpMethod method, string path, object? body = null)
    {
        var http = factory.CreateClient("Api");
        using var request = new HttpRequestMessage(method, path);
        if (body is not null) request.Content = JsonContent.Create(body, options: ApiJson.Options);

        HttpResponseMessage res;
        try
        {
            res = await http.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw new ApiException(0, $"Backend nicht erreichbar ({http.BaseAddress}).");
        }

        if (!res.IsSuccessStatusCode) throw new ApiException((int)res.StatusCode, await ErrorMessageAsync(res));
        return res.Content.Headers.ContentLength == 0 || res.StatusCode == System.Net.HttpStatusCode.NoContent
            ? default
            : await res.Content.ReadFromJsonAsync<T>(ApiJson.Options);
    }

    private static async Task<string> ErrorMessageAsync(HttpResponseMessage res)
    {
        try
        {
            var body = await res.Content.ReadFromJsonAsync<JsonElement>();
            if (body.TryGetProperty("errors", out var errors))
                return string.Join(' ', errors.EnumerateObject().SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString())));
            if (body.TryGetProperty("detail", out var detail)) return detail.GetString() ?? "";
            if (body.TryGetProperty("title", out var title)) return title.GetString() ?? "";
        }
        catch (JsonException)
        {
        }
        return $"HTTP {(int)res.StatusCode}";
    }
}
