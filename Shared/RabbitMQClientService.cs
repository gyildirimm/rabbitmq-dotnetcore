using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Shared;
public class RabbitMQClientService : IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQClientService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        string connectionString = configuration.GetConnectionString("RabbitMQ") ?? string.Empty;
        ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));

        _connectionFactory = new ConnectionFactory() { Uri = new Uri(connectionString), DispatchConsumersAsync = true };
        _connection = _connectionFactory.CreateConnection();
        _channel = Connect();
    }

    public IModel Connect()
    {
        _connection ??= _connectionFactory.CreateConnection();

        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        _channel = _connection.CreateModel();

        return _channel;
    }

    public void Publish(QueueModel model)
    {
        if(model.Exchange.Properties is null)
        {
            IBasicProperties properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            model.Exchange.Properties = properties;
        }

        _channel.PublishQueueModel(model);
    }

    public void ExchangeDeclare(QueueModel model)
    {
        _channel.ExchangeDeclareQueueModel(model);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();

        _connection?.Close();
        _connection?.Dispose();
    }
}
