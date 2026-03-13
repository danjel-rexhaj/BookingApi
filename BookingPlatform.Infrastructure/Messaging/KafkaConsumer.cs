using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BookingPlatform.Application.Interfaces;

namespace BookingPlatform.Infrastructure.Messaging;

public class KafkaConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public KafkaConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(async () =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "booking-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("booking-events");

            Console.WriteLine("Kafka Consumer running...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(1));

                    if (result == null)
                        continue;

                    Console.WriteLine("Kafka message: " + result.Message.Value);

                    using var scope = _scopeFactory.CreateScope();

                    var notificationService =
                        scope.ServiceProvider.GetRequiredService<INotificationService>();

                    await notificationService.SendNotificationAsync(
                        Guid.NewGuid(),
                        "Kafka booking event received"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Kafka error: " + ex.Message);
                }
            }
        });

        return Task.CompletedTask;
    }
}