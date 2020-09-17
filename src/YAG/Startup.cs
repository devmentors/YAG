using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using YAG.Messaging;

namespace YAG
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReverseProxy()
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"));
            services.AddMessaging();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseMessaging();
            app.Use(async (ctx, next) =>
            {
                const int retries = 3;
                var logger = ctx.RequestServices.GetRequiredService<ILogger<Startup>>();
                await Policy.Handle<HttpRequestException>()
                    .WaitAndRetryAsync(retries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                        , (exception, duration, retryAttempt, context) => logger.LogInformation($"Retry {retryAttempt}/{retries}"))
                    .ExecuteAsync(async () => await next());
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("", ctx => ctx.Response.WriteAsync("YAG"));
                endpoints.MapReverseProxy();
            });
        }
    }
}
