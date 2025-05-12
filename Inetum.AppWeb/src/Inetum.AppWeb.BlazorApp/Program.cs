using Inetum.AppWeb.BlazorApp.Components;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents(configure: options => options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = builder.Environment.IsDevelopment(); });
builder.Services.AddRadzenComponents();
// Con esta línea se añade el acceso a la configuración desde el contenedor de dependencias
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddHttpClient("InetumApi", client =>
{
    client.BaseAddress = new(builder.Configuration.GetSection("Api:Url").Value!);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
