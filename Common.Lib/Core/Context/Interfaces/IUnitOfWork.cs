using Common.Lib.Authentication;
using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public interface IUnitOfWork : IDisposable
    {
        INotificationHandler NotificationHandler { get; set; }
        TimeInfoLog? TimeInfoLog { get; set; }
        List<IUoWActInfo> UowActions { get; set; }

        void AddEntityDelete(Entity entity);
        void AddEntitySave(Entity entity);


        Task<IActionResult> CommitAsync(IEnumerable<IUoWActInfo>? actions = default, AuthInfo? info = null, ITraceInfo? trace = null);
    }
}
