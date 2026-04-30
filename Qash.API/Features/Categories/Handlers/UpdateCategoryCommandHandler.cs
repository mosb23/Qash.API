using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Categories.Commands;
using Qash.API.Features.Categories.DTOs;
using Qash.API.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Categories.Handlers;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.ApplicationUserId == request.UserId, cancellationToken);

        if (category is null)
        {
            return ApiResponse<CategoryDto>.FailResponse(
                "Update category failed.",
                ["Category was not found."]);
        }

        var name = request.Name.Trim();

        var duplicateExists = await _context.Categories
            .AnyAsync(x =>
                x.Id != request.Id &&
                x.ApplicationUserId == request.UserId &&
                x.Name.ToLower() == name.ToLower() &&
                x.Type == request.Type,
                cancellationToken);

        if (duplicateExists)
        {
            return ApiResponse<CategoryDto>.FailResponse(
                "Update category failed.",
                ["Another category with the same name and type already exists."]);
        }

        category.Name = name;
        category.Type = request.Type;
        category.Icon = request.Icon?.Trim();
        category.Color = request.Color?.Trim();
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.SuccessResponse(
            _mapper.Map<CategoryDto>(category),
            "Category updated successfully.");
    }
}