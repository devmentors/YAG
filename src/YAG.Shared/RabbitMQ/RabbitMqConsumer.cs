using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace YAG.Shared.RabbitMQ
{
    internal class RabbitMqConsumer : IHostedService
    {
        private readonly RabbitMqClient _rabbitMqClient;
        private readonly IOptions<RabbitMqOptions> _options;

        public RabbitMqConsumer(RabbitMqClient rabbitMqClient, IOptions<RabbitMqOptions> options)
        {
            _rabbitMqClient = rabbitMqClient;
            _options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.Value.Subscriptions is null)
            {
                return Task.CompletedTask;
            }

            foreach (var subscription in _options.Value.Subscriptions)
            {
                _rabbitMqClient.Subscribe(subscription.Queue, subscription.Exchange, subscription.RoutingKey);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _rabbitMqClient.Disconnect();

            return Task.CompletedTask;
        }
    }
}