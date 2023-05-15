using Core.Notifications;
using Core.Notifications.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KeyCloak.Services.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IDomainNotifier _notifier;

        public BaseController(IDomainNotifier notifier)
        {
            _notifier = notifier;
        }

        protected ActionResult CustomResponse(object? result = null)
        {
            if (HasNotification())
            {
                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "messages", _notifier.GetNotifications().Select(n => n.Value).ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                NotifyError(string.Empty, error.ErrorMessage);
            }

            return CustomResponse();
        }        

        protected bool HasNotification()
        {
            return !_notifier.HasNotification();
        }

        protected void NotifyError(string key, string message)
        {
            _notifier.Add(new DomainNotification(key, message));
        }
    }
}
