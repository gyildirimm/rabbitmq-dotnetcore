using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text;

namespace Shared;
public static class RabbitMQExtensions
{

    public static void ExchangeDeclareQueueModel(this IModel channel, QueueModel model)
    {
        channel.ExchangeDeclare(model.ExchangeName, type: model.Exchange.Type, durable: model.Exchange.Durable, autoDelete: model.Exchange.AutoDelete, arguments: model.Exchange.Arguments);
    }

    public static void PublishQueueModel(this IModel channel, QueueModel model)
    {
        channel.ExchangeDeclareQueueModel(model);
        channel.DeclareAndBindQueueForPublish(model);

        byte[] messageBody = Array.Empty<byte>();

        if (!string.IsNullOrEmpty(model.MessageBody)) messageBody = Encoding.UTF8.GetBytes(model.MessageBody);

        channel.BasicPublish(model.ExchangeName, model.PublishRoutingKey, model.Exchange.Properties, messageBody);
    }

    public static void QueueBindQueueModel(this IModel channel, QueueModel model)
    {
        foreach (var queueItem in model.Queues)
        {
            string queueName = queueItem.QueueName;

            if (string.IsNullOrEmpty(queueName))
            {
                queueName = channel.QueueDeclare().QueueName;
                queueItem.QueueName = queueName;
            }

            channel.QueueBind(queueName, model.ExchangeName, queueItem.RoutingKey, queueItem.Arguments);
        }
    }

    public static void BasicQosQueueModel(this IModel channel, QueueModel model)
    {
        channel.BasicQos((uint)model.QosConfig.PreFetchSize, (ushort)model.QosConfig.PreFetchCount, model.QosConfig.Global);
    }

    public static void SetRabbitMQLifeTime(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMQClientService>();
    }

    private static void QueueDeclareAndBindCustomModel(this IModel channel, QueueConfig config, string exchangeName)
    {
        string queueName = config.QueueName;
        if (string.IsNullOrEmpty(queueName)) return;

        channel.QueueDeclare(queueName, durable: config.Durable, exclusive: config.Exclusive, autoDelete: config.AutoDelete, arguments: config.Arguments);

        channel.QueueBind(queueName, exchangeName, config.RoutingKey, arguments: config.Arguments);
    }

    private static void DeclareAndBindQueueForPublish(this IModel channel, QueueModel model)
    {
        foreach (var item in model.Queues.Where(f => !string.IsNullOrEmpty(f.QueueName) ))
        {
            channel.QueueDeclareAndBindCustomModel(item, model.ExchangeName);
        }
    }
}
