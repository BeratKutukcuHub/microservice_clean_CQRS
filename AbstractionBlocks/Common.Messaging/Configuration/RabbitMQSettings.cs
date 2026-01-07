namespace AbstractionBlocks.Common.Messaging.Configuration;

public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "microservices.events";
    public string ExchangeType { get; set; } = "topic";
    public bool ExchangeDurable { get; set; } = true;
    public bool ExchangeAutoDelete { get; set; } = false;
    public string QueuePrefix { get; set; } = string.Empty;
    public bool QueueDurable { get; set; } = true;
    public bool QueueExclusive { get; set; } = false;
    public bool QueueAutoDelete { get; set; } = false;
    public ushort PrefetchCount { get; set; } = 10;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 1000;
    public bool EnableDeadLetterExchange { get; set; } = true;
    public string DeadLetterExchangeName { get; set; } = "microservices.events.dlx";
    public int MessageTTL { get; set; } = 86400000; // 24 hours in milliseconds
}
