using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    

    public interface ICrudHandler<T> where T : Entity, new()
    {
        Task<ISaveResult<T>> AddAsync(T entity);

        Task<ISaveResult<T>> UpdateAsync(T entity);

        Task<IDeleteResult> DeleteAsync(Guid id);
    }

}
