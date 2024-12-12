using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using CqrsDemo.Infrastructure.Caching;
using CqrsDemo.Domain.Entities;
using Polly;
using Polly.Retry;
using CqrsDemo.Infrastructure.Persistence;

namespace CqrsDemo.Infrastructure.Messaging
{
    public class RabbitMQConsumerService : BackgroundService, IAsyncDisposable
    {
        private readonly RabbitMQConnectionManager _connectionManager;
        private readonly ReadDbContext _readDbContext;
        private readonly AsyncRetryPolicy _retryPolicy;

        public RabbitMQConsumerService(RabbitMQConnectionManager connectionManager, ReadDbContext readDbContext)
        {
            _connectionManager = connectionManager;
            _readDbContext = readDbContext;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"🔄 RabbitMQ Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
                    });
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var channel = await _connectionManager.GetChannelAsync();
                var queues = new List<(string QueueName, string ExchangeName, string RoutingKey)>
                {
                    ("order-processing-queue", "order-exchange", "order.created"),
                    ("order-processing-queue", "order-exchange", "order.updated"),
                    ("order-processing-queue", "order-exchange", "order.deleted"),
                };

                foreach (var (queueName, exchangeName, routingKey) in queues)
                {
                    // Declare exchange and queue dynamically
                    await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);
                    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                    await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);

                    // Create a consumer for the queue
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        await _retryPolicy.ExecuteAsync(async () =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine($"[x] Received message from {queueName}: {message}");

                            var order = System.Text.Json.JsonSerializer.Deserialize<Order>(message);
                            if (order == null)
                            {
                                throw new Exception("Invalid message received");
                            }

                            switch (ea.RoutingKey)
                            {
                                case "order.created":
                                    _readDbContext.Add(order);
                                    await _readDbContext.SaveChangesAsync();
                                    break;
                                case "order.updated":
                                    var cachedOrder = await _readDbContext.FindAsync<Order>(order.Id);
                                    if (cachedOrder == null)
                                    {
                                        throw new Exception("Order not found");
                                    }
                                    cachedOrder.UpdateWithModifiedDate(order.Name, order.Price, order.ModifiedDate);
                                    await _readDbContext.SaveChangesAsync();
                                    break;
                                case "order.deleted":
                                    var cachedOrderToDelete = await _readDbContext.FindAsync<Order>(order.Id);
                                    if (cachedOrderToDelete == null)
                                    {
                                        throw new Exception("Order not found");
                                    }
                                    _readDbContext.Remove(order.Id);
                                    await _readDbContext.SaveChangesAsync();
                                    break;
                                default:
                                    throw new Exception("Invalid key");
                            }

                            await Task.CompletedTask; // Simulate async processing
                        });
                    };

                    // Start consuming messages
                    await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
                }
            });
        }

        public async ValueTask DisposeAsync()
        {
            if (_connectionManager != null)
            {
                await _connectionManager.DisposeAsync();
            }

        }
    }
}
