using BlazingPizza;
using BlazingPizza.Components;
using BlazingPizza.Components.Account;
using BlazingPizza.Data;
using BlazingPizza.Orders;
using BlazingPizza.Pizzas;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Add(OrderContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(PizzaContext.Default);
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<OrderDataService>();

builder.Services.AddHttpClient(
    "",
    client => client.BaseAddress = new("https://localhost:7099"));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContext<PizzaStoreContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<PizzaStoreUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<PizzaStoreContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<PizzaStoreUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Initialize the database
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();
    if (db.Database.EnsureCreated())
    {
        await SeedData.InitializeAsync(db);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazingPizza.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapOrderEndpoints();
app.MapPizzaEndpoints();

app.Run();
