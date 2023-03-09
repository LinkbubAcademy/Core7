using Common.Lib.Core.Context;
using Common.Lib.Infrastructure.Actions;
using Test.Lib.Models;

namespace Test.Lib.Context
{
    public interface IPostRepository : IRepository<Post>
    {
        ActionResult CleanEvilPostAsync();
    }
}
