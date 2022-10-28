using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LinksValidator
{
    class NetworkClient
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<(string, System.Net.HttpStatusCode)> SendRequest(Uri url)
        {
            using HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return ("", response.StatusCode);
            }

            using var sr = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1"));
            return (sr.ReadToEnd(), response.StatusCode);
        }
    }
}
