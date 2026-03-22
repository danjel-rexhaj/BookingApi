using System.Threading.Tasks;
using BookingPlatform.Application.Events;

namespace BookingPlatform.Application.Interfaces;

public interface IEventProducer
{
    Task PublishAsync(string topic, IntegrationEvent integrationEvent);
}