using System;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace ReviewApi.AuthenticationHelper
{
    class Helper
    {
        /// <summary>
        /// Extract token from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>token</returns>
        public static string GetTokenFromRequest(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return request.Headers.FirstOrDefault(h => h.Key == "Authorization").Value;
        }

        /// <summary>
        /// Send request to Auth0
        /// </summary>
        /// <param name="urlExtension"></param>
        /// <param name="method"></param>
        /// <param name="token"></param>
        /// <returns>The response of the request</returns>
        public async static Task<IRestResponse> Sendrequest(string urlExtension, Method method, string token)
        {
            const string baseUrl = "http://20.45.0.16/Authentication";
            var client = new RestClient(baseUrl + urlExtension);
            client.Timeout = -1;
            var request = new RestRequest(method);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", token);
            IRestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine("response.Content");
            Console.WriteLine(response.Content);
            return response;
        }

    }
}