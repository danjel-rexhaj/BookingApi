using BookingPlatform.Application.Interfaces;
using MediatR;

namespace BookingPlatform.Application.Properties.Search;

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
            request.City,
            request.Guests,
            request.PropertyType,
            request.StartDate,
            request.EndDate,
            request.SortBy,
            request.Page,
            request.PageSize);

        var result = new List<PropertyDto>();

        foreach (var p in properties)
        {
            var reviews = await _reviewRepository
                .GetReviewsForPropertyAsync(p.Id);

            var averageRating = reviews.Any()
                ? reviews.Average(r => r.Rating)
                : 0;

            var totalReviews = reviews.Count;

            result.Add(new PropertyDto(
                p.Id,
                p.Name,
                p.Description,
                p.Address.City,
                p.MaxGuests,
                p.PropertyType,
                averageRating,
                totalReviews
            ));
        }

        return result;
    }
}