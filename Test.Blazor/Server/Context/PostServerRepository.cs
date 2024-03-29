﻿using Common.Lib.Core.Context;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Server.Context;
using Test.Lib.Context;
using Test.Lib.Models;

namespace Test.Blazor.Server.Context
{
    public class PostServerRepository : ServerRepository<Post>, IPostRepository

    {
        public PostServerRepository(IDbSet<Post> dbSet, IWorkflowManager wfm, IContextFactory contextFactory) 
            : base(dbSet, wfm, contextFactory)
        {
        }

        public ActionResult CleanEvilPostAsync()
        {
            throw new NotImplementedException();
        }
    }
}
