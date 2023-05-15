using Core.Notifications.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Core.Notifications
{
    public class DomainNotifier : IDomainNotifier
    {
        private List<DomainNotification> _notifications;

        public DomainNotifier()
        {
            _notifications = new List<DomainNotification>();
        }

        public void Add(DomainNotification notification)
        {
            _notifications.Add(notification);
        }

        public List<DomainNotification> GetNotifications()
        {
            return _notifications;
        }

        public bool HasNotification()
        {
            return _notifications.Any();
        }
    }
}
