using Newtonsoft.Json;
using ParkyWeb.Repository.IRepository;
using System.Text;
using System.Net;
using System.Net.Http.Headers;

namespace ParkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public Repository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateAsync(string url, T objToCreate, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if(objToCreate != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(objToCreate), Encoding.UTF8, "application/json");

            }
            else
            {
                return false;
            }

            var client = _httpClientFactory.CreateClient();
            if(token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.SendAsync(request);
            return (response.StatusCode == HttpStatusCode.Created ? true : false);
        }

        public async Task<bool> DeleteAsync(string url, int Id, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url + Id);
            var client = _httpClientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.SendAsync(request);
            return response.StatusCode == HttpStatusCode.NoContent ? true : false;

        }

        public async Task<IEnumerable<T>> GetAllAsync(string url, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = _httpClientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = client.Send(request);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonContent);  
            }
            return null;
        }

        public async Task<T> GetAsync(string url, int Id, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url + Id);
            var client = _httpClientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonContent);
            }
            return null;
        }

        public async Task<bool> UpdateAsync(string url, T objToUpdate, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            if (objToUpdate != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(objToUpdate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }

            var client = _httpClientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.SendAsync(request);
            return (response.StatusCode == HttpStatusCode.NoContent ? true : false);
        }
    }
}
