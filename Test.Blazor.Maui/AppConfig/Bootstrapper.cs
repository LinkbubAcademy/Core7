using Common.Lib.Authentication;
using Common.Lib.Client.Services;
using Common.Lib.Core.Context;
using Common.Lib.Services.ParamsCarriers;
using Common.Lib.Services;
using Microsoft.AspNetCore.Components;
using Test.Lib.Models;
using Test.Lib.Context;
using System;
using Test.Lib.Metadata;

namespace Test.Blazor.Client.AppConfig
{
    public class Bootstrapper
    {
        MauiAppBuilder Builder { get; set; }
        public Bootstrapper(MauiAppBuilder builder)
        {
            Builder = builder;
            TestMetadataHandler.InitMetadata();
        }

        public void RegisterDependencies()
        {
            var url = "https://localhost:7168/";
            Builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(url) });
            Builder.Services.AddTransient<IServiceInvoker>(sp => new ServiceInvoker(
                                                                    url,
                                                                    sp.GetService<IContextFactory>()));

            Builder.Services.AddTransient<IParamsCarrierFactory, Common.Lib.Services.Protobuf.ParamsCarrierFactory>();
            Builder.Services.AddSingleton<IContextFactory>(sp => new ContextFactory(sp));

            RegisterDbSets();
            RegisterRepositories();
        }

        void RegisterRepositories()
        {
            Builder.Services.AddTransient<IRepository<Person>, GenericRepository<Person>>();
            Builder.Services.AddTransient<IRepository<Post>, GenericRepository<Post>>();
        }

        void RegisterDbSets()
        {
            Builder.Services.AddTransient<IDbSet<Person>, ClientDbSet<Person>>();
            Builder.Services.AddTransient<IDbSet<Post>, ClientDbSet<Post>>();
        }
    }
}
