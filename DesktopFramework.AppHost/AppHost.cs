var builder = DistributedApplication.CreateBuilder(args);

// Backend content API (in-memory for v1).
var api = builder.AddProject<Projects.DesktopFramework_Api>("api");

// Blazor desktop shell. Resolves the API by its logical name ("api") via
// service discovery — no hardcoded URLs. WaitFor avoids calling the API
// before it is ready.
builder.AddProject<Projects.DesktopFramework_Web>("web")
       .WithReference(api)
       .WaitFor(api)
       .WithExternalHttpEndpoints();

builder.Build().Run();
