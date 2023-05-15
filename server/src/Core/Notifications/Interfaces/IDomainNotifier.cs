using System.Collections.Generic;

namespace Core.Notifications.Interfaces
{
    public interface IDomainNotifier
    {
        void Add(DomainNotification notification);

        List<DomainNotification> GetNotifications();

        bool HasNotification();
    }
}
