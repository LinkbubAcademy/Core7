namespace Common.Lib.Core.Context
{
    public class GenericUoWRepository<T> : GenericRepository<T> where T : Entity, new()
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public override IDbSet<T> DbSet => UnitOfWork.GetDbSet<T>();

        public override IWorkflowManager WorkflowManager => UnitOfWork.WorkflowManager;

        public GenericUoWRepository()
            : base(null, null)
        {

        }
    }
}
