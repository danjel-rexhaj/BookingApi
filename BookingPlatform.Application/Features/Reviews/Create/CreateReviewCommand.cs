using BookingPlatform.Application.Features.Reviews.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Reviews.Create;

public record CreateReviewCommand(
    Guid BookingId,
    int Rating,
    string Comment
) : IRequest<Guid>;
