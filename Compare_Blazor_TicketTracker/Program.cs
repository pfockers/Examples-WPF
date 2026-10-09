using Compare_Blazor_TicketTracker;
using Compare_Blazor_TicketTracker.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Konfiguration kommt aus wwwroot/appsettings.json (Gegenstueck zu .env bzw. environment.ts).
var apiUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5300";

builder.Services.AddTransient<AuthHeaderHandler>();
builder.Services.AddHttpClient("Api", c => c.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<TicketService>();

await builder.Build().RunAsync();
