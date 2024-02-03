using Common.Lib.Core.Context;

namespace Common.Lib.DataAccess.EFCore
{
    public class UowNotificationHandler : INotificationHandler
    {
        public List<Action> PendingNotifications { get; set; } = [];

        public IContextFactory ContextFactory { get; set; }

        public void Handle(Action notificationAction)
        {
            PendingNotifications.Add(notificationAction);
        }

        public void HandlerAllNotifications()
        {
            // todo: use multple threads to improce performance
            foreach (var notificationAction in PendingNotifications)
            {
                notificationAction();
            }
        }

        public void Dispose()
        {
            PendingNotifications.Clear();
            ContextFactory.Dispose();
        }
    }
}
