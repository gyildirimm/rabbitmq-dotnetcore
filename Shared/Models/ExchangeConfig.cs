using RabbitMQ.Client;

namespace Shared;

public class ExchangeConfig
{
    public bool Durable { get; set; } = true;

    public bool AutoDelete { get; set; } = false;

    public string Type { get; set; }

    public IBasicProperties? Properties { get; set; }

    public IDictionary<string, object>? Arguments { get; set; }

    public ExchangeConfig(string type, bool durable = true, bool autoDelete = false, IBasicProperties? properties = null, IDictionary<string, object>? arguments = null)
    {
        Type = type;
        Durable = durable;
        AutoDelete = autoDelete;
        Properties = properties;
        Arguments = arguments;
    }
}
