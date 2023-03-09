namespace Common.Lib.Core.Context
{
    public interface IWorkflowManager
    {
        public Task SendNotificationsAsync(IEnumerable<Action> notificationAction);


    }
}
