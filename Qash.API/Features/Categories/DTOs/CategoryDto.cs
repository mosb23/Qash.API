using Qash.API.Domain.Enums;
using System;

namespace Qash.API.Features.Categories.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public string? Icon { get; set; }

    public string? Color { get; set; }
}