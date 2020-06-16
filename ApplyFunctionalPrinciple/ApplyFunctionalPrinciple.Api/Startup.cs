using ApplyFunctionalPrinciple.Logic.Model;
using ApplyFunctionalPrinciple.Logic.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplyFunctionalPrinciple.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddControllers();

            serviceCollection.AddScoped<UnitOfWork>();
            serviceCollection.AddTransient<IEmailGateway, EmailGateway>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.IsDevelopment())
                applicationBuilder.UseDeveloperExceptionPage();

            applicationBuilder.UseRouting();

            applicationBuilder.UseEndpoints(endpointRouteBuilder => { endpointRouteBuilder.MapControllers(); });
        }
    }
}