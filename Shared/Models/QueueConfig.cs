namespace Shared;

public class QueueConfig
{
    public string QueueName { get; set; }

    public string RoutingKey { get; set; }

    public bool Durable { get; set; } = true;

    public bool Exclusive { get; set; } = false;

    public bool AutoDelete { get; set; } = false;

    public IDictionary<string, object>? Arguments { get; set; }

    public QueueConfig(string queueName = "", string routingKey = "", bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object>? arguments = null)
    {
        QueueName = queueName ?? string.Empty;
        RoutingKey = routingKey ?? string.Empty;
        Durable = durable;
        Exclusive = exclusive;
        AutoDelete = autoDelete;
        Arguments = arguments;
    }
}
