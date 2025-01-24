using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Infrastructure.IntegrationEvents.Settings;

namespace SharedKernel.Infrastructure.IntegrationEvents.MessageBroker;

public interface IMessageBrokerClient
{
    void Publish(string exchangeName, string eventTypeName, string correlationId, byte[] body);
    void Subscribe(string queueName, Action<string, BasicDeliverEventArgs> callback);

    void DeadLetter(BasicDeliverEventArgs eventArgs);

    void Requeue(BasicDeliverEventArgs eventArgs);

    void Success(BasicDeliverEventArgs eventArgs);

}

public class RabbitMQClient : IMessageBrokerClient
{
    private const string _version = "1.0.0";
    private readonly ILogger<RabbitMQClient> _logger;
    private readonly CancellationTokenSource _cts;

    private readonly MessageBrokerSettings _messageBrokerOptions;
    private readonly MessageBrokerPublisherSettings _messageBrokerPublisherOptions;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    protected EventingBasicConsumer? _consumer;
    public RabbitMQClient(
        ILogger<RabbitMQClient> logger,
        IOptions<MessageBrokerSettings> messageBrokerOptions,
        IOptions<MessageBrokerPublisherSettings> messageBrokerPublisherOptions)
    {
        _logger = logger;
        _cts = new CancellationTokenSource();
        _messageBrokerOptions = messageBrokerOptions.Value;
        _messageBrokerPublisherOptions = messageBrokerPublisherOptions.Value;

        _logger.LogInformation($"Connecting to RabbitMQ: {_messageBrokerOptions.Connection.HostName}:{_messageBrokerOptions.Connection.Port}/{_messageBrokerOptions.Connection.VirtualHost}...");
        IConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = _messageBrokerOptions.Connection.HostName,
            Port = _messageBrokerOptions.Connection.Port,
            UserName = _messageBrokerOptions.Connection.UserName,
            Password = _messageBrokerOptions.Connection.Password,
            VirtualHost = _messageBrokerOptions.Connection.VirtualHost
        };

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        foreach (var exchange in _messageBrokerOptions.Exchanges)
        {
            var arguments = new Dictionary<string, object>();

            // Declare the dead letter exchange
            string deadLetterExchangeName, deadLetterQueueName;
            CreateExchange(exchange, ExchangeType.Fanout, "dead-letter", arguments, out deadLetterExchangeName, out deadLetterQueueName);

            string unRoutedExchangeName, unRoutedQueueName;
            CreateExchange(exchange, ExchangeType.Fanout, "un-routed", arguments, out unRoutedExchangeName, out unRoutedQueueName);

            // Declare exchange
            arguments.Clear();
            arguments.Add("alternate-exchange", unRoutedExchangeName);
            _channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Type == "Direct" ? ExchangeType.Direct : ExchangeType.Fanout,
                durable: exchange.Durable,
                arguments: arguments);

            var currentQueueName = string.Empty;
            foreach (var queue in exchange.Queues)
            {

                if (currentQueueName != queue.Name)
                {
                    // Set up the main queue with dead letter exchange settings
                    arguments.Clear();
                    arguments.Add("x-dead-letter-exchange", deadLetterExchangeName);

                    // Add specific arguments if specified in configuration

                    // Create queue
                    _channel.QueueDeclare(
                        queue: queue.Name,
                        durable: queue.Durable,
                        exclusive: queue.Exclusive,
                        autoDelete: queue.AutoDelete,
                        arguments: arguments);
                }

                _channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchange.Name,
                    routingKey: queue.RoutingKey);

                currentQueueName = queue.Name;
            }
        }

    }

    private void CreateExchange(MessageBrokerSettingsExchange exchange, string exchangeType, string suffix, IDictionary<string, object> arguments, out string exchangeName, out string queueName)
    {
        exchangeName = $"{exchange.Name}.{suffix}";
        _channel.ExchangeDeclare(exchangeName, exchangeType, durable: true, arguments: arguments);

        // Declare the dead letter queue
        queueName = $"{exchange.Name}-queue.{suffix}";
        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Bind the dead letter queue to the dead letter exchange
        _channel.QueueBind(
            queue: queueName,
            exchange: exchangeName,
            routingKey: string.Empty);
    }

    public void Publish(string exchangeName, string messageType, string correlationId, byte[] body)
    {
        IBasicProperties props = _channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.DeliveryMode = 2;

        props.Headers = new Dictionary<string, object>();
        props.Headers.Add("publisher", _messageBrokerPublisherOptions.Publisher);
        props.Headers.Add("correlationId", correlationId);
        props.Headers.Add("messageType", messageType);
        props.Headers.Add("timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        props.Headers.Add("version", _version);

        _channel.BasicPublish(exchangeName, messageType, props, body);
    }

    public void Subscribe(string queueName, Action<string, BasicDeliverEventArgs> callback)
    {
        if (_consumer is null)
            _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            callback(message, ea);
        };

        _channel.BasicConsume(queueName, autoAck: false, _consumer);
    }

    public void DeadLetter(BasicDeliverEventArgs eventArgs)
    {
        _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
    }

    public void Requeue(BasicDeliverEventArgs eventArgs)
    {
        _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
    }

    public void Success(BasicDeliverEventArgs eventArgs)
    {
        _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}

