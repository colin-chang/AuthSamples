using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ColinChang.IdentityServer.ClientCredentialConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection(nameof(IdentityServerOptions))
                .Get<IdentityServerOptions>();

            using var client = new HttpClient();
            //发现IdentityServer配置
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = options.Address,
                // Policy = new DiscoveryPolicy
                // {
                //     RequireHttps = false,
                //     ValidateEndpoints = false,
                //     ValidateIssuerName = false
                // }
            });
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //获取Token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Scope = options.Scope
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            //API调用
            using var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var response = await apiClient.GetAsync(options["WeatherApi"]);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));

            Console.ReadKey();
        }
    }
}