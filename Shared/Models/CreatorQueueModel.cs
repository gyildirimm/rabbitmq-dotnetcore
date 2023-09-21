using RabbitMQ.Client;

namespace Shared.Models;
public static class CreatorQueueModel
{
    public static QueueModel GetFanoutModel()
    {
        string exchangeName = "logs-fanout";

        QosConfig qosConfig = new QosConfig(0, 1, false);
        ExchangeConfig exchangeConfig = new ExchangeConfig(ExchangeType.Fanout);
        QueueConfig queueConfig = new QueueConfig();

        QueueModel model = new(exchangeName, exchange: exchangeConfig, qosConfig: qosConfig, queues: new List<QueueConfig>() { queueConfig });

        return model;
    }

    public static QueueModel GetDirectModel()
    {
        string exchangeName = "logs-direct";

        QosConfig qosConfig = new QosConfig(0, 1, false);
        ExchangeConfig exchangeConfig = new ExchangeConfig(ExchangeType.Direct);
        QueueConfig queueConfig = new QueueConfig();

        List<QueueConfig> queues = new List<QueueConfig>();

        Enum.GetNames(typeof(LogNames)).ToList().ForEach(f =>
        {
            string queueName = $"direct-queue-{f}";
            string routeKey = $"route-{f}";

            queues.Add(new QueueConfig(queueName, routeKey));
        });

        QueueModel model = new(exchangeName, exchange: exchangeConfig, qosConfig: qosConfig, queues: queues);

        return model;
    }

    public static QueueModel GetTopicModel()
    {
        string exchangeName = "logs-topic";

        QosConfig qosConfig = new QosConfig(0, 1, false);
        ExchangeConfig exchangeConfig = new ExchangeConfig(ExchangeType.Topic);

        List<QueueConfig> queues = new List<QueueConfig>()
        {
             new QueueConfig("", "Info.#"),
             new QueueConfig("", "*.*.Warning"),
             new QueueConfig("", "*.Error.*")
        };        

        QueueModel model = new(exchangeName, exchange: exchangeConfig, qosConfig: qosConfig, queues: queues);

        return model;
    }

    public static QueueModel GetHeaderModel(IModel channel)
    {
        string exchangeName = "header-exchange";

        QosConfig qosConfig = new QosConfig(0, 1, false);

        Dictionary<string, object> headers = new Dictionary<string, object>();
        headers.Add("format", "pdf");
        headers.Add("shape", "a4");

        IBasicProperties properties =  channel.CreateBasicProperties();
        properties.Headers = headers;
        properties.Persistent = true;

        ExchangeConfig exchangeConfig = new ExchangeConfig(ExchangeType.Headers, properties: properties);

        headers.Add("x-match", "all");
        List<QueueConfig> queues = new List<QueueConfig>()
        {
           new QueueConfig("", arguments: headers)
        };

        QueueModel model = new(exchangeName, exchange: exchangeConfig, qosConfig: qosConfig, queues: queues);

        return model;
    }
}
