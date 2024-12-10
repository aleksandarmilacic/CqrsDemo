using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsDemo.Infrastructure.Messaging
{
    public class RabbitMQConsumerService : BackgroundService, IAsyncDisposable
    {
        private readonly RabbitMQConnectionManager _connectionManager;

        public RabbitMQConsumerService(RabbitMQConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }
 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var channel = await _connectionManager.GetChannelAsync();
            await channel.QueueDeclareAsync("hello", false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received {message}");
                await Task.CompletedTask; // Simulate async processing
            };

            await channel.BasicConsumeAsync("order-processing-queue", true, consumer, cancellationToken: stoppingToken);

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
