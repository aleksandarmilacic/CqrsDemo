using CqrsDemo.Infrastructure.Messaging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Services
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync<T>(string exchangeName, string routingKey, T message);
    }

    public class RabbitMQPublisher : IRabbitMQPublisher, IAsyncDisposable
    {
        private readonly RabbitMQConnectionManager _connectionManager;
        private readonly AsyncPolicy _retryPolicy;

        public RabbitMQPublisher(RabbitMQConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
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

        public async Task PublishAsync<T>(string exchangeName, string routingKey, T message)
        {

            await _retryPolicy.ExecuteAsync(async () =>
            {
                var channel = await _connectionManager.GetChannelAsync();
                await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));


                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    body: body
                );

                Console.WriteLine($"📢 [PUBLISHED] Exchange: {exchangeName}, RoutingKey: {routingKey}, Message: {JsonSerializer.Serialize(message)}");
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
