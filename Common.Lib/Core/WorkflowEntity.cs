using Common.Lib.Core.Context;
using Common.Lib.Infrastructure.Actions;

namespace Common.Lib.Core
{
    public class WorkflowEntity : Entity
    {
        #region Save

        public override async Task<SaveResult> SaveAsync<T>(IUnitOfWork? uow = null)
        {
            return await base.SaveAsync<T>(uow);
        }

        #endregion
    }
}
