
namespace Common.Lib.Core.Context
{
    public class WorkflowManager : IWorkflowManager
    {
        public IContextFactory ContextFactory { get; set; }

        public void Dispose()
        {
        }

        public Task SendNotificationsAsync(IEnumerable<Action> notificationAction)
        {
            throw new NotImplementedException();
        }
    }
}