namespace BookingPlatform.Infrastructure.Messaging;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = default!;
    public string BookingTopic { get; set; } = default!;
}