using Common.Lib.Infrastructure;

namespace Common.Lib.Core.Context
{
    public class GenericUoWRepository : IUnitOfWork 
    {
        public IUnitOfWork UnitOfWork { get; set; }

        //public override IDbSet<T> DbSet => UnitOfWork.GetDbSet<T>();

        //public override IWorkflowManager WorkflowManager => UnitOfWork.WorkflowManager;

        public List<UoWActInfo> UowActions { get; set; } = new();

        public IWorkflowManager WorkflowManager { get; set; }

        public GenericUoWRepository()
            //: base(null, null, null)
        {

        }

        public void AddEntitySave(Entity entity)
        {
            throw new NotImplementedException();
        }

        public IDbSet<T1>? GetDbSet<T1>() where T1 : Entity, new()
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> CommitAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
