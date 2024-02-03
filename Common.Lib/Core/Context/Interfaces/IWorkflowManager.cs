namespace Common.Lib.Core.Context
{
    public interface IWorkflowManager : IContextElement
    {
        public Task SendNotificationsAsync(IEnumerable<Action> notificationAction);

    }
}
