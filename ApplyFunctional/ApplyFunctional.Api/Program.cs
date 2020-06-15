using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ApplyFunctional.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webHostBuilder => { webHostBuilder.UseStartup<Startup>(); })
                .Build()
                .Run();
        }
    }
}