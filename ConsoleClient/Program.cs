using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
   class Program
   {
      public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

      private static async Task MainAsync()
      {
         while (true)
         {
            Console.WriteLine("1: Client Credentials 2: Resouce Owner Password 0: Exit");
            var choice = Console.ReadLine();
            if (!string.IsNullOrEmpty(choice))
            {
               switch (choice.First().ToString().ToUpper())
               {
                  case "0":
                     return;
                  case "1":
                     await CredentialAuthentication();
                     break;
                  case "2":
                     await ResourceOwnerAuthentication();
                     break;
               }
            }
         }
      }

      private static async Task ResourceOwnerAuthentication()
      {
         Console.Write("User: ");
         var user = Console.ReadLine();
         Console.Write("Password: ");
         var password = Console.ReadLine();

         // discover endpoints from metadata
         var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
         if (disco.IsError)
         {
            Console.WriteLine(disco.Error);
            return;
         }

         // request token
         var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
         var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(user, password, "api1");

         if (tokenResponse.IsError)
         {
            Console.WriteLine(tokenResponse.Error);
            return;
         }

         Console.WriteLine(tokenResponse.Json);

         // call api
         var client = new HttpClient();
         client.SetBearerToken(tokenResponse.AccessToken);

         var response = await client.GetAsync("http://localhost:5001/identity");
         if (!response.IsSuccessStatusCode)
         {
            Console.WriteLine(response.StatusCode);
         }
         else
         {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
         }
      }

      private static async Task CredentialAuthentication()
      {
         // discover endpoints from metadata
         var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
         if (disco.IsError)
         {
            Console.WriteLine(disco.Error);
            return;
         }

         // request token
         var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
         var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

         if (tokenResponse.IsError)
         {
            Console.WriteLine(tokenResponse.Error);
            return;
         }

         Console.WriteLine(tokenResponse.Json);

         // call api
         var client = new HttpClient();
         client.SetBearerToken(tokenResponse.AccessToken);

         var response = await client.GetAsync("http://localhost:5001/identity");
         if (!response.IsSuccessStatusCode)
         {
            Console.WriteLine(response.StatusCode);
         }
         else
         {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(JArray.Parse(content));
         }

      }
   }
}