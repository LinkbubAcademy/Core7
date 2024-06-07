using Common.Lib.Authentication;
using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    

    public interface ICrudHandler<T> where T : Entity, new()
    {
        Task<ISaveResult<T>> AddAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null);

        Task<ISaveResult<T>> UpdateAsync(T entity, AuthInfo? info = null, ITraceInfo? trace = null);

        Task<IDeleteResult> DeleteAsync(Guid id, AuthInfo? info = null, ITraceInfo? trace = null);
    }

}
