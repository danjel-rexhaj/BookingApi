using Confluent.Kafka;
using System.Text.Json;
using BookingPlatform.Application.Interfaces;

namespace BookingPlatform.Infrastructure.Messaging;

public class KafkaProducer : IEventProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task SendBookingCreatedAsync(object bookingEvent)
    {
        var message = JsonSerializer.Serialize(bookingEvent);

        await _producer.ProduceAsync(
            "booking-events",
            new Message<Null, string>
            {
                Value = message
            });
    }
}