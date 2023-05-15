using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace KeyCloak.Services.Api.Extensions
{
    public static class PollyExtension
    {
        public static AsyncRetryPolicy<HttpResponseMessage> WaitTry()
        {
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                //Policy
                //.HandleResult<HttpResponseMessage>(r => (r.StatusCode == HttpStatusCode.InternalServerError) || 
                //                                        (r.StatusCode == HttpStatusCode.NotFound))
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                }, onRetry: (exception, retryCount, context) => {
                    Console.WriteLine($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                });

            return retry;
        }
    }
}
