using System.Text.Json;
using BookingPlatform.Application.Events;
using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BookingPlatform.Infrastructure.Messaging;

namespace BookingPlatform.Infrastructure.Outbox;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public OutboxPublisherService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OutboxPublisherService> logger,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
                var producer = scope.ServiceProvider.GetRequiredService<IEventProducer>();

                var messages = await dbContext.Set<OutboxMessage>()
                    .Where(x => x.ProcessedOnUtc == null)
                    .OrderBy(x => x.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        IntegrationEvent? integrationEvent = message.Type switch
                        {
                            nameof(BookingCreatedIntegrationEvent) =>
                                JsonSerializer.Deserialize<BookingCreatedIntegrationEvent>(message.Payload),

                            nameof(BookingRejectedIntegrationEvent) =>
                                JsonSerializer.Deserialize<BookingRejectedIntegrationEvent>(message.Payload),

                            nameof(BookingConfirmedIntegrationEvent) =>
                                JsonSerializer.Deserialize<BookingConfirmedIntegrationEvent>(message.Payload),

                            nameof(BookingCanceledIntegrationEvent) =>
                                JsonSerializer.Deserialize<BookingCanceledIntegrationEvent>(message.Payload),

                            nameof(PropertyApprovedIntegrationEvent) =>
                                JsonSerializer.Deserialize<PropertyApprovedIntegrationEvent>(message.Payload),

                            nameof(PropertyRejectedIntegrationEvent) =>
                                JsonSerializer.Deserialize<PropertyRejectedIntegrationEvent>(message.Payload),

                            nameof(PropertySuspendedIntegrationEvent) =>
                                JsonSerializer.Deserialize<PropertySuspendedIntegrationEvent>(message.Payload),

                            nameof(BookingReminderScheduledIntegrationEvent) =>
                                JsonSerializer.Deserialize<BookingReminderScheduledIntegrationEvent>(message.Payload),

                            nameof(UserRegisteredIntegrationEvent) =>
                                JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(message.Payload),

                            nameof(PasswordChangedIntegrationEvent) =>
                                JsonSerializer.Deserialize<PasswordChangedIntegrationEvent>(message.Payload),

                            nameof(ReviewCreatedIntegrationEvent) =>
                                JsonSerializer.Deserialize<ReviewCreatedIntegrationEvent>(message.Payload),

                            nameof(OwnerProfileCreatedIntegrationEvent) =>
                            JsonSerializer.Deserialize<OwnerProfileCreatedIntegrationEvent>(message.Payload),
                            _ => null
                        };

                        if (integrationEvent is null)
                        {
                            message.Error = $"Unknown event type: {message.Type}";
                            message.ProcessedOnUtc = DateTime.UtcNow;
                            continue;
                        }

                        await producer.PublishAsync(_kafkaSettings.BookingTopic, integrationEvent);

                        message.ProcessedOnUtc = DateTime.UtcNow;
                        message.Error = null;
                    }
                    catch (Exception ex)
                    {
                        message.Error = ex.Message;
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while publishing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}