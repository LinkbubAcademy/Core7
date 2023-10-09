using Common.Lib.Core.Context;
using Common.Lib.Infrastructure.Actions;
using Test.Lib.Context;
using Test.Lib.Models;

namespace Test.Blazor.Client.Context
{
    public class PostClientRepository : GenericRepository<Post>, IPostRepository
    {
        public PostClientRepository(IDbSet<Post> dbSet, IWorkflowManager wfm, IContextFactory contextFactory) 
            : base(dbSet, wfm, contextFactory)
        {
        }

        public ActionResult CleanEvilPostAsync()
        {
            throw new NotImplementedException();
        }
    }
}
