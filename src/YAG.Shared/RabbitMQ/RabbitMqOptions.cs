using System.Collections.Generic;

namespace YAG.Shared.RabbitMQ
{
    internal class RabbitMqOptions
    {
        public string ConnectionName { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> HostNames { get; set; }
        public IEnumerable<SubscriptionOptions> Subscriptions { get; set; }

        internal class SubscriptionOptions
        {
            public string Queue { get; set; }
            public string Exchange { get; set; }
            public string RoutingKey { get; set; }
        }
    }
}