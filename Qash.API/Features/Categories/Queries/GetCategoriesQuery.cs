using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.Categories.DTOs;
using System;
using System.Collections.Generic;

namespace Qash.API.Features.Categories.Queries;

public class GetCategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>
{
    public Guid UserId { get; set; }

    public CategoryType? Type { get; set; }

    public GetCategoriesQuery(Guid userId, CategoryType? type)
    {
        UserId = userId;
        Type = type;
    }
}