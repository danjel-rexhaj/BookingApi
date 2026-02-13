using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure
{
    public static class infrastructureRegistration
    {
        public static void RegisterInInfrastructure(this IServiceCollection services, IConfiguration configuration) {


            return configuration;
    }
}
