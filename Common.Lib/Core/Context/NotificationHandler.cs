
namespace Common.Lib.Core.Context
{
    public class NotificationHandler : INotificationHandler
    {
        public IContextFactory ContextFactory { get; set; }

        public void Handle(Action notificationAction)
        {
            notificationAction();
        }
        public void Dispose()
        {
            ContextFactory.Dispose();
        }
    }
}
