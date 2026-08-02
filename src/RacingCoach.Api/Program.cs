using RacingCoach.Api.Components;
using RacingCoach.Api.Endpoints;
using RacingCoach.Api.Extensions;
using RacingCoach.Domain.Extensions;
using RacingCoach.Infrastructure.Extensions;
using RacingCoach.Providers.GT7;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDomain();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);
builder.Services.AddGT7Provider();

var app = builder.Build();

app.Services.MigrateDatabase();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapSessionEndpoints();
app.MapProviderEndpoints();

app.Run();
