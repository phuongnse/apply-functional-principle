using System.IO;
using ApplyFunctionalPrinciple.Logic.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ApplyFunctionalPrinciple.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            SessionFactory.Init(configurationRoot.GetConnectionString("DefaultConnectionString"));

            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); })
                .Build()
                .Run();
        }
    }
}