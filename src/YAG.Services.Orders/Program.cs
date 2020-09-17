using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using YAG.Shared.RabbitMQ;

namespace YAG.Services.Orders
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .ConfigureServices(services => services.AddRabbitMq())
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("", ctx => ctx.Response.WriteAsync("Orders service"));
                            endpoints.MapGet("orders", ctx => ctx.Response.WriteAsync("Orders list"));
                            endpoints.MapGet("orders/{id}", ctx => ctx.Response.WriteAsync("Order details"));
                            endpoints.MapPost("orders", ctx => ctx.Response.WriteAsync("Order created"));
                        });
                    }));
    }
}