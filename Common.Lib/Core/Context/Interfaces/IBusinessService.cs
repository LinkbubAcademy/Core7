using Common.Lib.Authentication;
using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public interface IBusinessService : IContextElement
    {
        AuthInfo? AuthInfo { get; set; }
        ITraceInfo? Trace { get; set; }
        Task<IProcessActionResult> CallMethodAsync(string action, string[] serializedValues, IUnitOfWork? uow = default);
    }
}
