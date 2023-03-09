using Common.Lib.Authentication;
using Common.Lib.Client.Services;
using Common.Lib.Core.Context;
using Common.Lib.Services.ParamsCarriers;
using Common.Lib.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Test.Lib.Models;
using Test.Blazor.Client.Context;
using Test.Lib.Context;
using Test.Lib.Metadata;

namespace Test.Blazor.Client.AppConfig
{
    public class Bootstrapper
    {
        WebAssemblyHostBuilder Builder { get; set; }
        public Bootstrapper(WebAssemblyHostBuilder builder)
        {
            Builder = builder;
            TestMetadataHandler.InitMetadata();
        }

        public void RegisterDependencies()
        {
            Builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Test.Blazor.ServerAPI"));
            Builder.Services.AddHttpClient("Test.Blazor.ServerAPI", client => client.BaseAddress = new Uri(Builder.HostEnvironment.BaseAddress));
            Builder.Services.AddTransient<IServiceInvoker>(sp => new ServiceInvoker(
                                                                    sp.GetRequiredService<NavigationManager>().BaseUri,
                                                                    sp.GetService<IContextFactory>()));

            Builder.Services.AddTransient<IParamsCarrierFactory, Common.Lib.Services.Protobuf.ParamsCarrierFactory>();
            Builder.Services.AddSingleton<IContextFactory>(sp => new ContextFactory(sp) { IsServerMode = false});

            RegisterDbSets();
            RegisterRepositories();
        }

        void RegisterRepositories()
        {
            Builder.Services.AddTransient<IRepository<Person>, GenericRepository<Person>>();
            Builder.Services.AddTransient<IRepository<Post>, PostClientRepository>();
            Builder.Services.AddTransient<IPostRepository, PostClientRepository>();
        }

        void RegisterDbSets()
        {
            Builder.Services.AddTransient<IDbSet<Person>, ClientDbSet<Person>>();
            Builder.Services.AddTransient<IDbSet<Post>, ClientDbSet<Post>>();
        }
    }
}
