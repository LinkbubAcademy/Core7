﻿using Common.Lib.Core.Context;
using Common.Lib.Services.ParamsCarriers;
using Common.Lib.Server.Context;
using Test.Lib.Models;
using Test.Lib.Context;
using Test.Blazor.Server.Context;

namespace Test.Blazor.Server.App
{
    public class Bootstrapper
    {
        WebApplicationBuilder Builder { get; set; }
        public Bootstrapper(WebApplicationBuilder builder)
        {
            Builder = builder;            
        }

        public void RegisterDependencies()
        {
            Builder.Services.AddTransient<IParamsCarrierFactory, Common.Lib.Services.Protobuf.ParamsCarrierFactory>();
            //Builder.Services.AddTransient<IServiceInvoker>(sp => new ServiceInvoker(
            //                                                        sp.GetRequiredService<NavigationManager>().BaseUri,
            //                                                        sp.GetService<IContextFactory>()));

            Builder.Services.AddTransient<IWorkflowManager>(sp => new WorkflowManager());
            Builder.Services.AddSingleton<IContextFactory>(sp => new ContextFactory(sp, "Test.Lib") { IsServerMode = true });

            RegisterDbSets();
            RegisterRepositories();
        }

        void RegisterRepositories()
        {
            Builder.Services.AddTransient<IRepository<Person>, ServerRepository<Person>>();
            Builder.Services.AddTransient<IRepository<Post>, PostServerRepository>();
            Builder.Services.AddTransient<IPostRepository, PostServerRepository>();
        }

        void RegisterDbSets()
        {
            Builder.Services.AddTransient<IDbSet<Person>, MemoryDbSet<Person>>();
            Builder.Services.AddTransient<IDbSet<Post>, MemoryDbSet<Post>>();
        }
    }
}
