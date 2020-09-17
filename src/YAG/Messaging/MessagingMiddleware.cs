using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using YAG.Shared.RabbitMQ;

namespace YAG.Messaging
{
    internal class MessagingMiddleware : IMiddleware
    {
        private readonly RabbitMqClient _rabbitMqClient;
        private readonly RouteMatcher _routeMatcher;
        private readonly IDictionary<string, List<MessagingOptions.EndpointOptions>> _endpoints;

        public MessagingMiddleware(RabbitMqClient rabbitMqClient, RouteMatcher routeMatcher,
            IOptions<MessagingOptions> messagingOptions)
        {
            _rabbitMqClient = rabbitMqClient;
            _routeMatcher = routeMatcher;
            _endpoints = messagingOptions.Value.Endpoints.GroupBy(e => e.Method.ToUpperInvariant())
                .ToDictionary(e => e.Key, e => e.ToList());
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!_endpoints.TryGetValue(context.Request.Method, out var endpoints))
            {
                await next(context);
                return;
            }

            foreach (var endpoint in endpoints)
            {
                var match = _routeMatcher.Match(endpoint.Path, context.Request.Path);
                if (match is null)
                {
                    continue;
                }
                
                var message = await new StreamReader(context.Request.Body).ReadToEndAsync();
                _rabbitMqClient.Publish(message, endpoint.Exchange, endpoint.RoutingKey);
                context.Response.StatusCode = StatusCodes.Status202Accepted;
                return;
            }

            await next(context);
        }
    }
}