using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Shared.Models;
using Shared;
using System.Text;

namespace WorkerService.BackgroundServices;
public class HeaderWorker : BackgroundService
{
    private readonly ILogger<HeaderWorker> _logger;
    private readonly RabbitMQClientService _rabbitmqClientService;
    private IModel _channel;
    private QueueModel queueModel;

    public HeaderWorker(
        ILogger<HeaderWorker> logger,
        RabbitMQClientService rabbitmqClientService)
    {
        ArgumentNullException.ThrowIfNull(rabbitmqClientService, nameof(rabbitmqClientService));
        _logger = logger;
        _rabbitmqClientService = rabbitmqClientService;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {

        _channel = _rabbitmqClientService.Connect();
        queueModel = CreatorQueueModel.GetHeaderModel(_channel);

        _channel.ExchangeDeclareQueueModel(queueModel);
        _channel.BasicQosQueueModel(queueModel);


        return base.StartAsync(cancellationToken);
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueBindQueueModel(queueModel);

        foreach (var item in queueModel.Queues)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(item.QueueName, false, consumer);

            consumer.Received += Consumer_Received;
        }

        return Task.CompletedTask;
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
        await Task.Delay(1000);
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());

        _logger.LogInformation($"HeaderMessage: {message}, Route: {@event.RoutingKey}");

        _channel.BasicAck(@event.DeliveryTag, false);
    }
}