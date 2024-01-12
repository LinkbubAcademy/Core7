using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public interface IBusinessService : IDisposable
    {
        IContextFactory ContextFactory { get; set; }

        Task<IProcessActionResult> CallMethodAsync(string action, string[] serializedValues, IUnitOfWork? uow = default);
    }
}
