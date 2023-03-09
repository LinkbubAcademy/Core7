namespace Common.Lib.Core.Context
{
    public interface IUnitOfWork
    {
        void AddEntitySave(Entity entity);
        IDbSet<T>? GetDbSet<T>() where T : Entity, new();

        IWorkflowManager WorkflowManager { get; }
    }
}
