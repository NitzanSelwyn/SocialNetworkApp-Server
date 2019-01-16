using Common.Configs;
using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace SocialProjectServer.Services
{
    public class HttpClientSender : IHttpClient
    {
        public HttpClientSender()
        {

        }

        public HttpClient client { get; set; }

        public Tuple<object, HttpStatusCode> GetRequest(string route)
        {
            using (client = new HttpClient())
            {
                InitHttpClient();
                try
                {
                    HttpResponseMessage response = client.GetAsync(client.BaseAddress + route).Result;
                    object result = response.Content.ReadAsAsync(typeof(object)).Result;
                    return new Tuple<object, HttpStatusCode>(result, response.StatusCode);
                }
                catch (AggregateException)
                {
                    return new Tuple<object, HttpStatusCode>(null, HttpStatusCode.ExpectationFailed);
                }

            }
        }

        public Tuple<object, HttpStatusCode> PostRequest(string route, object obj = null)
        {
            using (client = new HttpClient())
            {
                InitHttpClient();
                try
                {
                    HttpResponseMessage response = client.PostAsJsonAsync(client.BaseAddress + route, obj).Result;
                    object result = response.Content.ReadAsAsync(typeof(object)).Result;
                    return new Tuple<object, HttpStatusCode>(result, response.StatusCode);
                }
                catch (AggregateException)
                {
                    return new Tuple<object, HttpStatusCode>(null, HttpStatusCode.ExpectationFailed);
                }
            }
        }

        public void InitHttpClient()
        {
            client.BaseAddress = new Uri(MainConfigs.AuthServiceUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}