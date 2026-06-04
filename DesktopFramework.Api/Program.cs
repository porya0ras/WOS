using DesktopFramework.Api.Endpoints;
using DesktopFramework.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

// In-memory content providers (v1 — no database).
builder.Services.AddSingleton<AppContentProvider>();
builder.Services.AddSingleton<NotificationProvider>();

// Allow the Blazor Web app to call the API from the browser if WASM is ever enabled.
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

var api = app.MapGroup("/api");
api.MapAppsEndpoints();
api.MapContentEndpoints();
api.MapNotificationsEndpoints();

app.Run();
