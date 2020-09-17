using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace YAG.Shared.RabbitMQ
{
    public class RabbitMqClient
    {
        private readonly ConcurrentDictionary<int, IModel> _channels = new ConcurrentDictionary<int, IModel>();
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqClient> _logger;

        public RabbitMqClient(IConnection connection, ILogger<RabbitMqClient> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public void DeclareExchanges(params string[] exchanges)
        {
            using var channel = _connection.CreateModel();
            foreach (var exchange in exchanges)
            {
                channel.ExchangeDeclare(exchange, "topic");
            }

            channel.Close();
        }

        public void Publish(string message, string exchange, string routingKey)
        {
            using var channel = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.MessageId = Guid.NewGuid().ToString("N");
            properties.CorrelationId = Guid.NewGuid().ToString("N");
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Headers = new Dictionary<string, object>();
            channel.BasicPublish(exchange, routingKey, properties, body);
        }

        public void Subscribe(string queue, string exchange, string routingKey)
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue, true, false, false);
            channel.QueueBind(queue, exchange, routingKey);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.Span);
                _logger.LogInformation($"Received a message [ID: {args.BasicProperties.MessageId}]\n{message}");
                return Task.CompletedTask;
            };

            channel.BasicConsume(queue, true, consumer);
            _channels.TryAdd(channel.ChannelNumber, channel);
        }

        public void Disconnect()
        {
            foreach (var (id, channel) in _channels)
            {
                _logger.LogInformation($"Closing a channel {id}...");
                channel.Close();
            }

            _logger.LogInformation("Closing RabbitMQ connection...");
            _connection.Close();
        }
    }
}