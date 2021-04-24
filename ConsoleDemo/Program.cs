using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var hc = new HttpClient();
            await hc.GetAsync("https://localhost:5000/");

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}