using BookingPlatform.Application.Interfaces;
using BookingPlatform.Infrastructure.Persistence;
using BookingPlatform.Infrastructure.Persistence.Repositories;
using BookingPlatform.Infrastructure.Repositories;
using BookingPlatform.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BookingPlatform.Infrastructure.DependencyInjection;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpContextAccessor();

        // Core services
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        services.AddScoped<IEmailService, SendGridEmailService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPropertyRuleRepository, PropertyRuleRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IBlockedDateRepository, BlockedDateRepository>();
        services.AddScoped<ISeasonalPriceRepository, SeasonalPriceRepository>();
        services.AddScoped<IAmenityRepository, AmenityRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
        services.AddScoped<IPropertyAmenityRepository, PropertyAmenityRepository>();
        services.AddScoped<IOwnerProfileRepository, OwnerProfileRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        return services;
    }
}