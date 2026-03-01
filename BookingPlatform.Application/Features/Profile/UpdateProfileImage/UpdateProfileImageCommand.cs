using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookingPlatform.Application.Features.Profile.UpdateProfileImage;

public class UpdateProfileImageCommand : IRequest<Unit>
{
    public string ImageUrl { get; set; } = null!;
}
