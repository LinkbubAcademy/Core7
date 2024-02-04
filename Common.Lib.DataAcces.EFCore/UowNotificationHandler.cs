using Common.Lib.Core.Context;

namespace Common.Lib.DataAccess.EFCore
{
    public class UowNotificationHandler : INotificationHandler
    {
        public Dictionary<Guid, Action> PendingNotifications { get; set; } = [];

        public IContextFactory ContextFactory { get; set; }

        public void Handle(Guid entityId, Action notificationAction)
        {
            if (PendingNotifications.ContainsKey(entityId))
                PendingNotifications[entityId] = notificationAction;
            else
                PendingNotifications.Add(entityId, notificationAction);
        }


        public void DeleteNotification(Guid entityId)
        {
            if (PendingNotifications.ContainsKey(entityId))
                PendingNotifications.Remove(entityId);
        }

        public void HandlerAllNotifications()
        {
            // todo: use multple threads to improce performance
            foreach (var notificationAction in PendingNotifications.Values)
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
