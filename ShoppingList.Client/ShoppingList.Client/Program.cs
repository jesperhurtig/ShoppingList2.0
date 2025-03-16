using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShoppingList.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5281/api/") });

await builder.Build().RunAsync();