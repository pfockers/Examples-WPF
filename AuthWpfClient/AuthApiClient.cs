using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AuthWpfClient;

using System.Net.Http;

public sealed class AuthApiClient : IDisposable
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://localhost:7043")
    };

    public async Task<string> SignInAsync(string userName, string password)
    {
        this._httpClient.DefaultRequestHeaders.Authorization = null;
        using var response = await this._httpClient.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(userName, password));

        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();
            return $"Sign-in failed: {(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{details}";
        }

        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (login is null)
        {
            return "Sign-in failed: the API returned an invalid token response.";
        }

        this._httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(login.TokenType, login.AccessToken);
        return $"Signed in. Token expires at {login.ExpiresAtUtc.ToLocalTime():G}.";
    }

    public async Task<string> GetAsync(string path)
    {
        using var response = await this._httpClient.GetAsync(path);
        var content = await response.Content.ReadAsStringAsync();
        return response.IsSuccessStatusCode
            ? content
            : $"Request failed: {(int)response.StatusCode} {response.ReasonPhrase}{Environment.NewLine}{content}";
    }

    public void SignOut() => this._httpClient.DefaultRequestHeaders.Authorization = null;

    public void Dispose() => this._httpClient.Dispose();

    private sealed record LoginRequest(string UserName, string Password);

    private sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);
}
