using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace YAG.Services.Products
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
                        endpoints.MapGet("", ctx => ctx.Response.WriteAsync("Products service"));
                        endpoints.MapGet("products", ctx => ctx.Response.WriteAsync("Products list"));
                        endpoints.MapGet("products/{id}", ctx => ctx.Response.WriteAsync("Product details"));
                    });
                }));
    }
}