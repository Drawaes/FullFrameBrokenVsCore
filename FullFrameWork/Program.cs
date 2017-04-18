using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FullFrameWork
{
    class Program
    {
        public static X509Certificate2 Certificate = new X509Certificate2(@"TestCert.pfx", "Test123t");
        public static bool WithHttps = false;
        static string address;
        static void Main(string[] args)
        {
            address = "http://localhost:7777";
            if (WithHttps)
            {
                address = "https://localhost:7777";
            }

            TestFilterBeforeHttpsFact();

            TestConnecting();

            Console.ReadLine();
        }

        public static void TestConnecting()
        {
            var client = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (request, cert, chain, policy) =>
                {
                    return true;
                }
            });
            
            var result = client.GetAsync(address);
            result.Wait();
            var actualResult = result.Result.Content.ReadAsStringAsync();
            actualResult.Wait();
            var expectedResult = string.Join(",", Enumerable.Repeat("testingtesting", 100));
            Console.WriteLine($"Result was {expectedResult == actualResult.Result}");
            Debug.Assert(expectedResult == actualResult.Result);

        }

        public static void TestFilterBeforeHttpsFact()
        {
            var host = new WebHostBuilder()
                .UseKestrel((ops) =>
                {
                    ops.Switcheroo();
                    if (WithHttps)
                    {
                        ops.UseHttps(Certificate);
                    }
                })
                .UseUrls(address)
                .UseStartup<Startup>()
                .Build();
            host.Start();
        }

        public class Startup
        {
            public void Configure(IApplicationBuilder app) =>
                app.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync(string.Join(",", Enumerable.Repeat("testingtesting", 100)));
                    return;
                });
        }
    }
}