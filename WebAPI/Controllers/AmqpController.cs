using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Shared;
using Shared.Models;
using System.Text;
using System.Threading.Channels;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AmqpController : ControllerBase
{
    private readonly RabbitMQClientService _rabbitmqClientService;

    public AmqpController(RabbitMQClientService rabbitmqClientService)
    {
        ArgumentNullException.ThrowIfNull(rabbitmqClientService, nameof(rabbitmqClientService));

        _rabbitmqClientService = rabbitmqClientService;
    }

    [HttpPost("Fanout")]
    public IActionResult Fanout()
    {
        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {

            string message = $"log {x}";

            //var messageBody = Encoding.UTF8.GetBytes(message);

            QueueModel model = CreatorQueueModel.GetFanoutModel();
            model.MessageBody = message;
            
            _rabbitmqClientService.Publish(model);

            //channel.BasicPublish("logs-fanout", "", null, messageBody);

            Console.WriteLine($"Mesaj gönderilmiştir : {message}");

        });

        return Ok();
    }


    [HttpPost("Direct")]
    public IActionResult Direct()
    {
        QueueModel model = CreatorQueueModel.GetDirectModel();
        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            LogNames log = (LogNames)new Random().Next(1, 5);
            string message = $"log-type: {log}";
            model.MessageBody = message;
            model.PublishRoutingKey = $"route-{log}";

            _rabbitmqClientService.Publish(model);

            Console.WriteLine($"Mesaj gönderilmiştir : {message}");
        });

        return Ok();
    }

    [HttpPost("Topic")]
    public IActionResult Topic()
    {
        QueueModel model = CreatorQueueModel.GetTopicModel();
        Random rnd = new Random();
        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            LogNames log1 = (LogNames)rnd.Next(1, 5);
            LogNames log2 = (LogNames)rnd.Next(1, 5);
            LogNames log3 = (LogNames)rnd.Next(1, 5);

            string message = $"log-type: {log1}-{log2}-{log3}";
            model.PublishRoutingKey = $"{log1}.{log2}.{log3}";

            model.MessageBody = message;

            _rabbitmqClientService.Publish(model);

            Console.WriteLine($"Mesaj gönderilmiştir : {message}");
        });

        return Ok();
    }

    [HttpPost("Header")]
    public IActionResult Header(string headerMessage = "")
    {
        IModel channel = _rabbitmqClientService.Connect();
        QueueModel model = CreatorQueueModel.GetHeaderModel(channel);

        string message = headerMessage ?? "Empty Message";
        model.MessageBody = message;

        _rabbitmqClientService.Publish(model);
        Console.WriteLine($"Mesaj gönderilmiştir : {message}");

        return Ok();
    }

}
