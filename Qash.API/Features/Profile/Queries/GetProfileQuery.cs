using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Profile.DTOs;
using System;

namespace Qash.API.Features.Profile.Queries;

public class GetProfileQuery : IRequest<ApiResponse<ProfileDto>>
{
    public Guid UserId { get; set; }

    public GetProfileQuery(Guid userId)
    {
        UserId = userId;
    }
}