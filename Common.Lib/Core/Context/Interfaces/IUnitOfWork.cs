using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public interface IUnitOfWork : IDisposable
    {
        public List<IUoWActInfo> UowActions { get; set; }

        void AddEntityDelete(Entity entity);
        void AddEntitySave(Entity entity);


        Task<IActionResult> CommitAsync(IEnumerable<IUoWActInfo>? actions = default);
    }
}
