using System.Text.Json;
using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace BookingPlatform.Infrastructure.Messaging;

public class KafkaProducer : IEventProducer
{
    private readonly KafkaSettings _settings;

    public KafkaProducer(IOptions<KafkaSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task PublishAsync(string topic, IntegrationEvent integrationEvent)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        var message = new Message<string, string>
        {
            Key = integrationEvent.EventId.ToString(),
            Value = JsonSerializer.Serialize(integrationEvent)
        };

        await producer.ProduceAsync(topic, message);
    }
}