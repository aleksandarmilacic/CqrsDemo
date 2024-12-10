using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using CqrsDemo.Infrastructure.Caching;
using CqrsDemo.Domain.Entities;

namespace CqrsDemo.Infrastructure.Messaging
{
    public class RabbitMQConsumerService : BackgroundService, IAsyncDisposable
    {
        private readonly RabbitMQConnectionManager _connectionManager;
        private readonly RedisCache _redisCache;

        public RabbitMQConsumerService(RabbitMQConnectionManager connectionManager, RedisCache redisCache)
        {
            _connectionManager = connectionManager;
            _redisCache = redisCache;
        }
 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var channel = await _connectionManager.GetChannelAsync();
            var queues = new List<(string QueueName, string ExchangeName, string RoutingKey)>
            {
                ("order-processing-queue", "order-exchange", "order.created"),
                ("payment-processing-queue", "payment-exchange", "payment.completed"),
                ("reporting-queue", "report-exchange", "report.generated")
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
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[x] Received message from {queueName}: {message}");


                    
                    var order = System.Text.Json.JsonSerializer.Deserialize<Order>(message);
                    if(order == null)
                    {
                        throw new Exception("Invalid message received");
                    }

                    var cachedOrder = await _redisCache.GetAsync<Order>($"order:{order.Id}");
                    if (cachedOrder == null)
                    {
                        await _redisCache.SetAsync($"order:{order.Id}", order);
                    }


                    await Task.CompletedTask; // Simulate async processing
                };

                // Start consuming messages
                await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
            }
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
