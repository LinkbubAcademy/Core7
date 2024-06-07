using Common.Lib.Core.Context;

namespace Common.Lib.DataAccess.EFCore
{
    public class UowNotificationHandler : INotificationHandler
    {
        public Queue<Tuple<Guid, Action>> PendingNotifications { get; set; } = [];

        public IContextFactory ContextFactory { get; set; }

        public void Handle(Guid entityId, Action notificationAction)
        {
            PendingNotifications.Enqueue(new Tuple<Guid, Action>(entityId, notificationAction));
        }


        public void DeleteNotification(Guid entityId)
        {
            var actionsToKeep = new List<Tuple<Guid, Action>>();

            foreach (var notification in PendingNotifications)
            {
                if (notification.Item1 != entityId)
                    actionsToKeep.Add(notification);
            }
             
            PendingNotifications = new Queue<Tuple<Guid, Action>>(actionsToKeep);
        }

        public void HandlerAllNotifications()
        {
            // todo: use multple threads to improce performance

            while (PendingNotifications.Count > 0)
            {
                var current = PendingNotifications.Dequeue(); // removed
                                                              // use Queue.Peek() if you want to look at it witout removing it
                                                              // Do stuff
                current.Item2();
            }
        }

        public void Dispose()
        {
            PendingNotifications.Clear();
            ContextFactory.Dispose();
        }
    }
}
