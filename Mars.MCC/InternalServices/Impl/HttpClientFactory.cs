using System;
using System.Net.Http;

namespace Mars.MCC.InternalServices.Impl
{
    internal class HttpClientFactory : IHttpClientFactory
    {
        private readonly string baseUrl;

        public HttpClient CreateClient(string name)
        {
            return new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public HttpClientFactory(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
    }
}
