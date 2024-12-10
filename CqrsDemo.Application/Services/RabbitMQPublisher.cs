using CqrsDemo.Infrastructure.Messaging;
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

        public RabbitMQPublisher(RabbitMQConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task PublishAsync<T>(string exchangeName, string routingKey, T message)
        {
            var channel = await _connectionManager.GetChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var basicProperties = new BasicProperties(); 

            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: basicProperties,
                body: body
            );

            Console.WriteLine($"📢 [PUBLISHED] Exchange: {exchangeName}, RoutingKey: {routingKey}, Message: {JsonSerializer.Serialize(message)}");
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
