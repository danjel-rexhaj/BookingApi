using BookingPlatform.Application.Interfaces;
using BookingPlatform.Domain.Entities;
using MediatR;

namespace BookingPlatform.Application.Features.Properties.Search;

public class GetPropertiesHandler
    : IRequestHandler<GetPropertiesQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository _repository;
    private readonly IReviewRepository _reviewRepository;

    public GetPropertiesHandler(
        IPropertyRepository repository,
        IReviewRepository reviewRepository)
    {
        _repository = repository;
        _reviewRepository = reviewRepository;
    }

    public async Task<List<PropertyDto>> Handle(
        GetPropertiesQuery request,
        CancellationToken cancellationToken)
    {
        var properties = await _repository.SearchAsync(
            request.Country,
            request.City,
            request.Guests,
            request.PropertyType,
            request.Amaneties,
            request.Rating,
            request.Price,
            request.StartDate,
            request.EndDate,
            request.SortBy
        );

        var result = new List<PropertyDto>();

        foreach (var property in properties)
        {
            var reviews = await _reviewRepository
                .GetReviewsForPropertyAsync(property.Id);

            double averageRating = 0;
            int totalReviews = 0;

            if (reviews.Any())
            {
                averageRating = reviews.Average(r => r.Rating);
                totalReviews = reviews.Count;
            }

            var dto = new PropertyDto(
                property.Id,
                property.Address.Country,
                property.Address.City,
                property.Name,
                property.Description,
                property.MaxGuests,
                property.PropertyType,
                property.BasePricePerNight,
                averageRating,
                totalReviews
            );

            result.Add(dto);
        }

        return result;
    }
}