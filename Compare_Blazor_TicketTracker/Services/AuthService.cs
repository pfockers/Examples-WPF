using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Compare_Blazor_TicketTracker.Services;

public record Session(string Token, string Username);

public class ApiException(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}

public static class ApiJson
{
    // Enums als "low", "in-progress", ... (wie in der API).
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower) },
    };
}

// Singleton: haelt die Anmeldung; wird auch vom HTTP-Handler benutzt (Gegenstueck zum Angular-Interceptor).
public class AuthService(IJSRuntime js, IHttpClientFactory factory, NavigationManager nav)
{
    private const string Key = "auth";
    private bool _initialized;

    public string? Token { get; private set; }
    public string? Username { get; private set; }
    public bool IsLoggedIn => Token is not null;
    public event Action? Changed;

    // sessionStorage ist nur ueber JS-Interop erreichbar, deshalb asynchron.
    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;
        var raw = await js.InvokeAsync<string?>("sessionStorage.getItem", Key);
        if (raw is not null && JsonSerializer.Deserialize<Session>(raw, ApiJson.Options) is { } s)
        {
            Token = s.Token;
            Username = s.Username;
        }
    }

    public async Task LoginAsync(string username, string password)
    {
        HttpResponseMessage res;
        try
        {
            res = await factory.CreateClient("Api").PostAsJsonAsync("api/auth/login", new { username, password });
        }
        catch (HttpRequestException)
        {
            throw new ApiException(0, "Backend nicht erreichbar.");
        }

        if (res.StatusCode == HttpStatusCode.Unauthorized) throw new ApiException(401, "Benutzername oder Passwort falsch.");
        if (!res.IsSuccessStatusCode) throw new ApiException((int)res.StatusCode, $"HTTP {(int)res.StatusCode}");

        var session = (await res.Content.ReadFromJsonAsync<Session>(ApiJson.Options))!;
        Token = session.Token;
        Username = session.Username;
        await js.InvokeVoidAsync("sessionStorage.setItem", Key, JsonSerializer.Serialize(session, ApiJson.Options));
        Changed?.Invoke();
    }

    public async Task LogoutAsync()
    {
        Token = null;
        Username = null;
        await js.InvokeVoidAsync("sessionStorage.removeItem", Key);
        Changed?.Invoke();
        nav.NavigateTo("login");
    }
}

// DelegatingHandler: haengt das Token an jede API-Anfrage und meldet bei 401 ab.
public class AuthHeaderHandler(AuthService auth) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var isLogin = request.RequestUri!.AbsolutePath.EndsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase);
        if (!isLogin && auth.Token is { } token)
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized && !isLogin)
            await auth.LogoutAsync();
        return response;
    }
}
