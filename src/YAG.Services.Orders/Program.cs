using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace YAG.Services.Orders
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("", ctx => ctx.Response.WriteAsync("Orders service"));
                        endpoints.MapGet("orders", ctx => ctx.Response.WriteAsync("Orders list"));
                        endpoints.MapGet("orders/{id}", ctx => ctx.Response.WriteAsync("Order details"));
                    });
                }));
    }
}