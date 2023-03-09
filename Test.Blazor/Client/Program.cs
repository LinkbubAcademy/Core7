using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Test.Blazor.Client;
using Test.Blazor.Client.AppConfig;
using Test.Lib.Metadata;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// Supply HttpClient instances that include access tokens when making requests to the server project
var bootstrapper = new Bootstrapper(builder);
bootstrapper.RegisterDependencies();

await builder.Build().RunAsync();
