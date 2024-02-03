using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public interface IBusinessService : IContextElement
    {
        Task<IProcessActionResult> CallMethodAsync(string action, string[] serializedValues, IUnitOfWork? uow = default);
    }
}
