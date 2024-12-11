using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;

namespace CqrsDemo.Infrastructure.Messaging
{
    public class RabbitMQConnectionManager : IAsyncDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;
        private AsyncPolicy _retryPolicy;

        public RabbitMQConnectionManager()
        {
            _factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? "rabbitmq",
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest"
            };

            _retryPolicy = Policy
               .Handle<BrokerUnreachableException>()
               .WaitAndRetryAsync(
                   retryCount: 5,
                   sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                   onRetry: (exception, timeSpan, retryCount, context) =>
                   {
                       Console.WriteLine($"🔄 RabbitMQ Connection Retry {retryCount} after {timeSpan.TotalSeconds} seconds due to: {exception.Message}");
                   });
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    _connection = await _factory.CreateConnectionAsync();
                }
                return _connection;
            });
        }

        public async Task<IChannel> GetChannelAsync()
        {
            if (_channel == null || _channel.IsClosed)
            {
                _channel = await (await GetConnectionAsync()).CreateChannelAsync();
            }
            return _channel;
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
        }
    }
}
