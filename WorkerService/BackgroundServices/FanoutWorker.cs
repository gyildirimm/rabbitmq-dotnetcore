using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using Shared.Models;
using System.Data;
using System.Text.Json;
using System.Text;
using System.Threading.Channels;

namespace WorkerService.BackgroundServices;
internal class FanoutWorker : BackgroundService
{
    private readonly ILogger<FanoutWorker> _logger;
    private readonly RabbitMQClientService _rabbitmqClientService;
    private IModel _channel;
    private QueueModel queueModel;

    public FanoutWorker(
        ILogger<FanoutWorker> logger, 
        RabbitMQClientService rabbitmqClientService)
    {
        ArgumentNullException.ThrowIfNull(rabbitmqClientService, nameof(rabbitmqClientService));
        _logger = logger;
        _rabbitmqClientService = rabbitmqClientService;

        queueModel = CreatorQueueModel.GetFanoutModel();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {

        _channel = _rabbitmqClientService.Connect();
        _channel.ExchangeDeclareQueueModel(queueModel);
        _channel.BasicQosQueueModel(queueModel);

        return base.StartAsync(cancellationToken);
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueBindQueueModel(queueModel);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        foreach (var item in queueModel.Queues)
        {
            _channel.BasicConsume(item.QueueName, false, consumer);

            consumer.Received += Consumer_Received;
        }

        return Task.CompletedTask;
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
    {
        await Task.Delay(2000);
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());
        _logger.LogInformation($"FanoutMessage: {message}");

        _channel.BasicAck(@event.DeliveryTag, false);
        //using (var scope = _serviceProvider.CreateScope())
        //{
        //    var context = scope.ServiceProvider.GetRequiredService<AdventureWorks2019Context>();

        //    products = context.Products.ToList();
        //}
    }
}
