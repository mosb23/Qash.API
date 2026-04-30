using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.Categories.DTOs;
using System;

namespace Qash.API.Features.Categories.Commands;

public class UpdateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public string? Icon { get; set; }

    public string? Color { get; set; }
}