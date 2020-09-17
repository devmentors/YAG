using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YAG.Shared.RabbitMQ;

namespace YAG.Messaging
{
    internal static class Extensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            }

            var section = configuration.GetSection("Messaging");
            var messagingOptions = new MessagingOptions();
            section.Bind(messagingOptions);
            if (!messagingOptions.Enabled)
            {
                return services;
            }

            services.AddSingleton<MessagingMiddleware>();
            services.AddSingleton<RouteMatcher>();
            services.Configure<MessagingOptions>(section);
            services.AddRabbitMq();

            return services;
        }

        public static IApplicationBuilder UseMessaging(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<MessagingOptions>>().Value;
            if (!options.Enabled)
            {
                return app;
            }

            app.UseMiddleware<MessagingMiddleware>();
            var rabbitMqClient = app.ApplicationServices.GetRequiredService<RabbitMqClient>();
            rabbitMqClient.DeclareExchanges(options.Endpoints.Select(e => e.Exchange).ToArray());

            return app;
        }
    }
}