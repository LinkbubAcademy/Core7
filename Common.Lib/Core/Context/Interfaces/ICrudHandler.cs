using Common.Lib.Core.Expressions;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core.Context
{
    

    public interface ICrudHandler<T> where T : Entity, new()
    {
        Task<ActionResult> AddAsync(T entity);

        Task<ActionResult> UpdateAsync(T entity);

        Task<ActionResult> DeleteAsync(Guid id);
    }

}
