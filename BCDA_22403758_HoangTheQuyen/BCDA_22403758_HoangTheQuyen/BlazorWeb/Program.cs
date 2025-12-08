using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWeb;
using BlazorWeb.Services;
using Blazored.LocalStorage;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP Client for API calls (localhost:5000)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5000")
});

// Blazor Bootstrap
builder.Services.AddBlazorBootstrap();

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

// Custom Services
builder.Services.AddScoped<ApiClient>();

await builder.Build().RunAsync();
