using Common.Lib.Authentication;
using Common.Lib.Client.Services;
using Common.Lib.Core.Context;
using Common.Lib.Core.Metadata;
using Common.Lib.Server.Context;
using Common.Lib.Server.Protobuf;
using Common.Lib.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.ResponseCompression;
using Test.Blazor.Server.App;
using Test.Lib.Metadata;

TestMetadataHandler.InitMetadata();

var builder = WebApplication.CreateBuilder(args);

// add gRPC service 
builder.Services.AddGrpc();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var bootstrapper = new Bootstrapper(builder);
bootstrapper.RegisterDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// must be added after UseRouting and before UseEndpoints 
app.UseGrpcWeb();

app.UseEndpoints(endpoints =>
{
    // map to and register the gRPC service
    endpoints.MapGrpcService<CoreServicesRequest>().EnableGrpcWeb();
});

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
