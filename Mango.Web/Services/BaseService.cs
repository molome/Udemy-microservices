using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        public ResponseDto responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            this.responseModel = new ResponseDto();
            this.httpClient = httpClientFactory;
        }

        public async Task<T> SendAsyn<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MAMangoAPI");
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Headers.Add("Accept", "application/json");
                httpRequestMessage.RequestUri = new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Clear();
                if(apiRequest.Data != null)
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),Encoding.UTF8,"application/json");
                }
                if(!string.IsNullOrEmpty(apiRequest.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",apiRequest.AccessToken);
                }

                HttpResponseMessage apiResponseMessage = null;
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        httpRequestMessage.Method=HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        httpRequestMessage.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        httpRequestMessage.Method = HttpMethod.Delete;
                        break;
                    default:
                        httpRequestMessage.Method = HttpMethod.Get;
                        break;
                }
                apiResponseMessage = await client.SendAsync(httpRequestMessage);
                var apiContent = await apiResponseMessage.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);
                return apiResponseDto;
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto()
                {
                    DisplayMessage = "Error",
                    ErrorMessages = new List<string>() { Convert.ToString(ex.Message) },
                    IsSuccess=false
                };
                var res = JsonConvert.SerializeObject(dto);
                var apiResponseDto = JsonConvert.DeserializeObject<T>(res);
                return apiResponseDto;
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
