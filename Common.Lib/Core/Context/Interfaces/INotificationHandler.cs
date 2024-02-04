namespace Common.Lib.Core.Context
{
    public interface INotificationHandler : IContextElement
    {
        void Handle(Guid entityId, Action notificationAction);
    }
}
