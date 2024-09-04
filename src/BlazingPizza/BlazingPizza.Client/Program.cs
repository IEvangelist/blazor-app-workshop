using BlazingPizza.Client;
using BlazingPizza.Client.Orders;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddTransient<CookieHandler>();

builder.Services.AddHttpClient<ApiClient>(
    client => client.BaseAddress = new(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped<OrderState>();

builder.Services.AddHttpClient<OrdersClient>(
    client => client.BaseAddress = new(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CookieHandler>();

await builder.Build().RunAsync();

public class CookieHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);

        return base.SendAsync(request, cancellationToken);
    }
}
