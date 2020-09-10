using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace YAG.Services.Users
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
                        endpoints.MapGet("", ctx => ctx.Response.WriteAsync("Users service"));
                        endpoints.MapGet("users", ctx => ctx.Response.WriteAsync("Users list"));
                        endpoints.MapGet("users/{id}", ctx => ctx.Response.WriteAsync("User details"));
                    });
                }));
    }
}