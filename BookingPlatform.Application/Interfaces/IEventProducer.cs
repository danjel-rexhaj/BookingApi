using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingPlatform.Application.Interfaces;

public interface IEventProducer
{
    Task SendBookingCreatedAsync(object bookingEvent);
}