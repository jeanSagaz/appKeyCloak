using KeyCloak.Services.Api.Exceptions;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KeyCloak.Services.Api.Services
{
    public abstract class Service
    {
        protected StringContent GetContent(object data)
        {
            return new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");
        }

        protected async Task<T> DeserializeResponseObject<T>(HttpResponseMessage responseMessage)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = await responseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(result))
                return default(T);

            return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        }

        protected bool HandlingErrorsResponse(HttpResponseMessage response)
        {
            switch ((int)response.StatusCode)
            {
                case 401:
                case 403:
                case 404:
                case 405:
                case 500:
                    //throw new CustomHttpRequestException(response.StatusCode);
                    return false;

                case 400:
                    return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        //protected ResponseResult ReturnOk()
        //{
        //    return new ResponseResult();
        //}
    }
}
