using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace YAG.Shared.RabbitMQ
{
    public static class Extensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            }

            var section = configuration.GetSection("RabbitMQ");
            services.Configure<RabbitMqOptions>(section);
            services.AddSingleton<RabbitMqClient>();
            services.AddHostedService<RabbitMqConsumer>();
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                var connectionFactory = new ConnectionFactory
                {
                    Port = options.Port,
                    VirtualHost = options.VirtualHost,
                    UserName = options.Username,
                    Password = options.Password,
                    DispatchConsumersAsync = true
                };

                return connectionFactory.CreateConnection(options.HostNames.ToList(), options.ConnectionName);
            });

            return services;
        }
    }
}