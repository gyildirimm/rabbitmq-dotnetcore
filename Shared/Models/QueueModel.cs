using RabbitMQ.Client;

namespace Shared;
public class QueueModel
{
    public string ExchangeName { get; set; }

    public string PublishRoutingKey { get; set; }

    public string MessageBody { get; set; }

    public ExchangeConfig Exchange { get; set; }

    public List<QueueConfig> Queues { get; set; }

    public QosConfig QosConfig { get; set; }

    public QueueModel(string exchangeName, string publishRoutingKey = "", string messageBody = "", ExchangeConfig? exchange = null, List<QueueConfig>? queues = null, QosConfig? qosConfig = null)
    {
        qosConfig ??= new QosConfig();
        queues ??= new List<QueueConfig>();
        exchange ??= new ExchangeConfig(exchangeName);

        ExchangeName = exchangeName;
        PublishRoutingKey = publishRoutingKey ?? string.Empty;
        MessageBody = messageBody;
        Exchange = exchange;
        Queues = queues;
        QosConfig = qosConfig;
    }
}
